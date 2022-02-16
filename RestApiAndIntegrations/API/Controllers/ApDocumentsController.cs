using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace AD.CAAPS.API.Controllers
{
    [Route("api/ap-documents")]
    [Produces("application/json")]
    [ApiController]
    public class ApDocumentsController : BaseController
    {
        const string controllerName = "AP-DOCUMENTS";

        private const int DEFAULT_FILESTREAM_BUFFER_SIZE = 4096;

        // GET: api/ap-documents/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(ApDocument), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetApDocument([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne<ApDocument>(MethodBase.GetCurrentMethod().Name, id, new ApDocumentServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/ap-documents
        [HttpPost]
        [ProducesResponseType(typeof(ApDocument), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostApDocument([FromBody] ApDocument apDocument, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POST";
            try
            {
                var apDocumentServices = new ApDocumentServices(GetDBConfiguration(GetClient(client)));
                logger.Debug($"{methodName}: Attempting to submit data to apDocumentServices.CreateApDocument().", apDocument);
                ApDocument result = await apDocumentServices.CreateApDocument(apDocument).ConfigureAwait(false);
                logger.Debug($"{methodName}: AP Document successfully created.", result);
                return CreatedAtAction(nameof(GetApDocument), new { id = result.ID }, result);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/ap-documents/5/files
        [HttpPost("{id}/files")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadApDocumentFile(IFormFile file, [FromRoute] int id, [FromQuery] string client = "")
        {
            string methodName = string.Join('-', nameof(ApDocumentsController), nameof(UploadApDocumentFile));
            
            if (file == null || file.Length == 0) return NotFound("No file uploaded.");
            
            try
            {
                client = GetClient(client);

                DBConfiguration dbConfiguration = GetDBConfiguration(client);
                var apDocumentServices = new ApDocumentServices(dbConfiguration);
                var fileNameServices = new FileNameServices(dbConfiguration);
                var fileLinkServices = new FileLinkServices(dbConfiguration);
                
                ApDocument apDocument = await apDocumentServices.GetOne(id).ConfigureAwait(false);
                if (apDocument == null) return NotFound($"Ap Document with id {id} not found");

                int fileCount = await fileLinkServices.FileLinkCount(id).ConfigureAwait(false);
                string fileName = (fileCount == 0) ? 
                                  string.Concat(apDocument.CaapsRecordId, CAAPSConstants.PDF_EXTENSION) :
                                  string.Concat(string.Join('_', apDocument.CaapsRecordId, fileCount), CAAPSConstants.PDF_EXTENSION);

                string serverFilePath = await GetServerFilePath(dbConfiguration).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(serverFilePath))
                    return NotFound($"Configuration for {CAAPSConstants.IMAGE_DIRECTORY} not found");

                string s3BucketName = await GetS3BucketName(serverFilePath, dbConfiguration).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(s3BucketName))
                    return NotFound($"Configuration for S3 Bucket not found");

                string s3BucketPath = BuildS3BucketPath(serverFilePath, Directory.GetDirectoryRoot(serverFilePath), s3BucketName);

                using (Amazon.S3.IAmazonS3 s3client = InitializeAmazonS3Client())
                    await AmazonS3Utils.UploadFileAsync(s3client, file.OpenReadStream(), s3BucketPath, fileName).ConfigureAwait(false);
                logger.Debug("File successfully uploaded");

                FileNameRecord fileNameRecord = await fileNameServices.CreateFileName(new FileNameRecord {
                    FilePath = serverFilePath,
                    FileName = fileName
                }).ConfigureAwait(false);
                logger.Debug("Successfully created a record in FILENAMES table", fileNameRecord);

                FileLink fileLinkRecord = await fileLinkServices.CreateFileLink(new FileLink {
                    RecordId = id,
                    FileId = fileNameRecord.ID,
                    FileIndex =  CAAPSConstants.DEFAULT_FILE_INDEX,
                    ManagedTableId = CAAPSConstants.DEFAULT_MANAGEDTABLEID
                }).ConfigureAwait(false);
                logger.Debug("Successfully linked CAAPS record and file from FILENAMES table", fileLinkRecord);

                var response = new ApDocumentResponse {
                    Id = id,
                    CaapsId = apDocument.CaapsRecordId,
                    Success = true,
                    FileName = fileName,
                    FileId = fileNameRecord.ID,
                    ServerFilePath = serverFilePath,
                    S3BucketPath = s3BucketPath
                };

                logger.Debug("Upload completed with no errors.", response);
                return Ok(response);
            }
            catch (Exception e)
            {
                return ConvertExceptionToHttpStatusCode(e, methodName);
            }
        }

        // GET: api/ap-documents/5/files/download
        [HttpGet("{id}/files/download")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(406)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadApDocumentFile([FromRoute] int id, [FromQuery] bool downloadMultiple = false, [FromQuery] string client = "")
        {
            string methodName = string.Join('-', nameof(ApDocumentsController), nameof(DownloadApDocumentFile));
            try
            {
                client = GetClient(client);

                DBConfiguration dbConfiguration = GetDBConfiguration(client);

                var fileLinkServices = new FileLinkServices(dbConfiguration);
                var fileNameServices = new FileNameServices(dbConfiguration);

                IQueryable<FileLink> fileLinks = fileLinkServices.GetFileLinks(id);
                int FileCount = await fileLinks.CountAsync().ConfigureAwait(false);
                if (FileCount <= 0)
                {
                    string message = $"{methodName}: No uploaded files for CAAPS record with id {id}";
                    logger.Error(message);
                    return NotFound(message);
                }

                if (!downloadMultiple || FileCount == 1)
                {
                    FileLink fileLink = await fileLinks.Where(t => t.FileIndex == 0).SingleOrDefaultAsync<FileLink>().ConfigureAwait(false);

                    if (fileLink == null)
                    {
                        string message = $"{methodName}: File link index not found for CAAPS record with id {id}";
                        logger.Error(message);
                        return NotFound(message);
                    }

                    FileNameRecord fileName = await fileNameServices.GetFileNameAsync(fileLink.FileId).ConfigureAwait(false);

                    if (fileName == null)
                    {
                        string message = $"{methodName}: File record not found for file with id {fileLink.FileId}";
                        logger.Error(message);
                        return NotFound(message);
                    }

                    string rootFilePath = Path.GetPathRoot(fileName.FilePath);
                    string s3BucketName = await GetS3BucketName(rootFilePath, dbConfiguration).ConfigureAwait(false);
                    string s3BucketFilePath = BuildS3BucketPath(fileName.FilePath, rootFilePath, s3BucketName);
                    logger.Debug($"S3 Bucket File Path: {s3BucketFilePath}");
                    string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Temp", fileName.FileName);
                    logger.Debug($"Temporary path: {tempPath}");
                    using (Amazon.S3.IAmazonS3 s3client = InitializeAmazonS3Client())
                       await AmazonS3Utils.DownloadFileAsync(s3client, tempPath, s3BucketFilePath, fileName.FileName).ConfigureAwait(false);
                    return File(new FileStream(tempPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, DEFAULT_FILESTREAM_BUFFER_SIZE, FileOptions.DeleteOnClose), "application/pdf", fileName.FileName);
                }
                else
                {
                    string groupedFolderName = string.Join("_", "Documents", id, DateTime.UtcNow.ToString("yyyyMMdd_hhmmss"));
                    string groupedFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Temp", groupedFolderName);
                    string zipFolderName = groupedFolderName + ".zip";
                    string zipFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Temp", zipFolderName);

                    foreach (FileLink fileLink in fileLinks)
                    {
                        FileNameRecord fileName = await fileNameServices.GetFileNameAsync(fileLink.FileId).ConfigureAwait(false);

                        if (fileName == null)
                        {
                            string message = $"{methodName}: File record not found for file with id {fileLink.FileId}";
                            logger.Error(message);
                            continue;
                        }

                        string rootFilePath = Path.GetPathRoot(fileName.FilePath);
                        string s3BucketName = await GetS3BucketName(rootFilePath, dbConfiguration).ConfigureAwait(false);
                        string s3BucketFilePath = BuildS3BucketPath(fileName.FilePath, rootFilePath, s3BucketName);
                        logger.Debug($"S3 Bucket File Path: {s3BucketFilePath}");
                        string tempPath = Path.Combine(groupedFolderPath, fileName.FileName);
                        logger.Debug($"Temporary path: {tempPath}");

                        using Amazon.S3.IAmazonS3 s3client = InitializeAmazonS3Client();
                        await AmazonS3Utils.DownloadFileAsync(s3client, tempPath, s3BucketFilePath, fileName.FileName).ConfigureAwait(false);
                    }
                    ZipFile.CreateFromDirectory(groupedFolderPath, zipFolderPath);

                    return File(new FileStream(zipFolderPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, DEFAULT_FILESTREAM_BUFFER_SIZE, FileOptions.DeleteOnClose), "application/octet-stream", zipFolderName);
                }
            }
            catch (Exception e)
            {
                return ConvertExceptionToHttpStatusCode(e, methodName);
            }
        }

        [NonAction]
        private static async Task<string> GetServerFilePath(DBConfiguration dbConfiguration)
        {
            var systemOptionServices = new SystemOptionServices(dbConfiguration);

            SystemOption systemOption = await systemOptionServices.GetSystemOptionByName(CAAPSConstants.IMAGE_DIRECTORY).ConfigureAwait(false);
            if (systemOption == null || (systemOption != null && string.IsNullOrWhiteSpace(systemOption.OptionValue)))
                return string.Empty;
            else
                return Path.Combine(systemOption.OptionValue, DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString("D2"), DateTime.UtcNow.Day.ToString("D2"));
        }

        [NonAction]
        private static async Task<string> GetS3BucketName(string serverFilePath, DBConfiguration dbConfiguration)
        {
            var s3DriveConfigurationServices = new S3DriveConfigurationServices(dbConfiguration);

            string serverRoot = Directory.GetDirectoryRoot(serverFilePath);
            string serverRoot2 = Utils.CleanRootFolder(Directory.GetDirectoryRoot(serverFilePath));

            S3DriveConfiguration s3DriveConfiguration = await s3DriveConfigurationServices.GetS3DriveConfigurationByPhysicalDriveToken(serverRoot).ConfigureAwait(false);

            if (s3DriveConfiguration == null)
                s3DriveConfiguration = await s3DriveConfigurationServices.GetS3DriveConfigurationByPhysicalDriveToken(serverRoot2).ConfigureAwait(false);

            if (s3DriveConfiguration == null || (s3DriveConfiguration != null && string.IsNullOrWhiteSpace(s3DriveConfiguration.S3Bucket)))
                return string.Empty;
            else
                return s3DriveConfiguration.S3Bucket;
        }

        [NonAction]
        private static string BuildS3BucketPath(string filePath, string originalRoot, string s3BucketName)
        {
            //replacing root folder with s3 bucket name
            filePath = filePath.Replace(originalRoot, s3BucketName + "/", StringComparison.InvariantCultureIgnoreCase);

            //replacing all \ characters into /
            filePath = filePath.Replace('\\', '/');

            //removing last / character in filePath, the filePath should only display like this acu-bucketname/DOCUMENTS/ADF2_CAAPS/LIVE/2020/01/01
            if (filePath.EndsWith('/'))
                filePath = filePath.Remove(filePath.Length - 1);

            return filePath;
        }
    }
}