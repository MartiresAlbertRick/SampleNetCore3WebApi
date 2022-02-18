using AD.CAAPS.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AD.CAAPS.Importer.Logic
{
    public static class FileReader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static LoadDataResponse<T> IfCsvThenRead<T>(string importFileName, Dictionary<string, string> dbToModelFieldMapping, string fieldDelimiter, string culture)
        {
            Utils.CheckObjectIsNullThrowException(dbToModelFieldMapping, () => new ArgumentNullException(nameof(dbToModelFieldMapping)));
            var response = new LoadDataResponse<T>();
            if (FileUtils.IsCsvFile(importFileName))
            {
                response.Data.AddRange((new ImportCsvReader<T>(
                            importFileName,
                            fieldDelimiter,
                            dbToModelFieldMapping,
                            culture
                        )).ReadFileToModel());
                response.Success = true;
            }
            return response;
        }

        public static LoadDataResponse<T> IfExcelThenRead<T>(string importFileName, Dictionary<string, string> dbToModelFieldMapping)
        {
            Utils.CheckObjectIsNullThrowException(dbToModelFieldMapping, () => new ArgumentNullException(nameof(dbToModelFieldMapping)));
            var result = new LoadDataResponse<T>();
            if (FileUtils.IsExcelFile(importFileName))
            {
                result.Data.AddRange((new ExcelReader<T>(
                            importFileName,
                            dbToModelFieldMapping
                        )).ReadFileToModel());
                result.Success = true;
            }
            return result;
        }

        public static List<string> RetrieveImportFilesFromFolder(string directoryName,
                                                          bool fileNameIsRegex,
                                                          string fileNamePattern,
                                                          bool matchCurrentDateAndFileCreationDate,
                                                          bool matchCurrentDateAndFileNameDate,
                                                          bool getTopOneFileByCreationDate,
                                                          string dateTimePattern)
        {
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(directoryName, () => new ArgumentException($"No value is set for argument {directoryName}"));

            var directoryInfo = new DirectoryInfo(directoryName);
            FileInfo[] files = directoryInfo.GetFiles()
                                            .OrderBy(t => t.CreationTime)
                                            .ToArray();
            var fileNames = new List<string>();

            if (getTopOneFileByCreationDate)
            {
                if (fileNameIsRegex)
                {
                    string fileName = files.Where(t => Regex.IsMatch(t.Name, fileNamePattern))
                                            .OrderByDescending(t => t.CreationTime)
                                            .First()
                                            .FullName;
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        return fileNames;
                    }
                    else
                    {
                        logger.Trace($"Adding filename to the list {fileName}");
                        fileNames.Add(fileName);
                    }
                }
                else
                {
                    // if fileNameIsRegex == false then we should assume that fileNamePattern is not a regex pattern e.g. VendorMaster.csv
                    fileNames.AddRange(GetFilesByStaticFileName(files, fileNamePattern));
                }
            }
            else
            {
                if (fileNameIsRegex)
                {
                    // if fileNameIsRegex == true then we should assume that fileNamePattern is a regex pattern
                    FileInfo[] filteredFileInfo = files.Where(t => Regex.IsMatch(t.Name, fileNamePattern)).ToArray();
                    foreach (FileInfo file in filteredFileInfo)
                    {
                        if (matchCurrentDateAndFileCreationDate)
                        {
                            if (FileUtils.MatchCurrentDateAndFileCreationDate(file))
                            {
                                logger.Trace($"Adding filename to the list {file.FullName}");
                                fileNames.Add(file.FullName);
                            }
                        }
                        else if (matchCurrentDateAndFileNameDate)
                        {
                            if (FileUtils.MatchCurrentDateAndFileNameDate(file, fileNamePattern, dateTimePattern))
                            {
                                fileNames.Add(file.FullName);
                            }
                        }
                        else
                        {
                            fileNames.Add(file.FullName);
                        }
                    }
                }
                else
                {
                    // if fileNameIsRegex == false then we should assume that fileNamePattern is not a regex pattern e.g. VendorMaster.csv
                    fileNames.AddRange(GetFilesByStaticFileName(files, fileNamePattern));
                }
            }
            return fileNames;
        }

        public static List<string> GetFilesByStaticFileName(FileInfo[] files, string fileName)
        {
            Utils.CheckObjectIsNullThrowException(files, () => new ArgumentNullException(nameof(files)));
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(fileName, () => new ArgumentException($"No value is set for argument {fileName}"));
            var fileNames = new List<string>();
            FileInfo[] filteredFileInfo = files.Where(t => t.Name == fileName).ToArray();
            foreach (FileInfo fileInfo in filteredFileInfo)
            {
                fileNames.Add(fileInfo.FullName);
            }
            return fileNames;
        }
    }
}
