using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AD.CAAPS.DocumentUploader
{
    public class Uploader
    {
        private readonly AppSettings appSettings;
        private readonly string docHeader;
        private readonly int records;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Uploader(int records, string docHeader, AppSettings appSettings)
        {
            if (records <= 0)
                throw new ConfigurationMissingException("No records count specified. Enter a record count greater than 0");
            if (string.IsNullOrWhiteSpace(docHeader))
                throw new ConfigurationMissingException("No docHeader specified. Enter a docHeader prefix. e.g. TEST");
            this.records = records;
            this.docHeader = docHeader;
            this.appSettings = appSettings;
        }


       /* private static string GetRestResponseErrorDetails(IRestResponse response)
        {
            if (response != null)
            {
                return $"Code: {(int)response.StatusCode}. Status: {response.StatusDescription}. Error: {response.ErrorMessage}. Response: {response.Content}";
            }
            else
            {
                return "Response undefined";
            }
        }*/

        public async Task<ExitCode> Start()
        {
            ExitCode exitCode = ExitCode.Successful;
            var request1 = new RestRequest(RestSharp.Method.POST);
            request1.AddHeader("X-Client-Id", appSettings.XClientId);
            request1.AddHeader("Content-Type", "application/json");
            request1.AddHeader("Ocp-Api-Trace", appSettings.OcpApiTrace);
            request1.AddHeader("Ocp-Apim-Subscription-Key", appSettings.OcpApimSubscriptionKey);

            appSettings.FileSource = Path.Combine(appSettings.FileSource, "test.PDF");
            var request2 = new RestRequest(RestSharp.Method.POST);
            request2.AddHeader("X-Client-Id", appSettings.XClientId);
            request2.AddHeader("Content-Type", "multipart/form-data");
            request2.AddHeader("Ocp-Api-Trace", appSettings.OcpApiTrace);
            request2.AddHeader("Ocp-Apim-Subscription-Key", appSettings.OcpApimSubscriptionKey);
            request2.AddFile("file", appSettings.FileSource);

            int success = 0;
            int failedOnPost = 0;
            int failedOnUpload = 0;

            Uri PostEndpoint = new Uri(appSettings.PostEndpoint);
            for (int i = 0; i < records; i++)
            {
                ApDocument apDocument = NewApDocument(i + 1);
                var p = new Parameter("application/json", JsonConvert.SerializeObject(apDocument), RestSharp.ParameterType.RequestBody);
                request1.AddParameter(p);
                try
                {
                    logger.Debug($"Posting document [{i + 1} of {records}]: {PostEndpoint}");

                    var utils1 = new RestSharpUtils(PostEndpoint, request1);
                    var response = await utils1.SendRequestAsync();

                    request1.Parameters.Remove(p);

                    if (RestSharpUtils.IsResposeSuccess(response))
                    {
                        ApDocument createdApDocument = JsonConvert.DeserializeObject<ApDocument>(response.Content);
                        if (createdApDocument == null)
                        {
                            logger.Error($"StatusCode Posting document [{i + 1} of {records}]: header id {apDocument.DocHeaderUID} not successful");
                            failedOnPost++;
                            continue;
                        }
                        else
                        {
                            logger.Info($"Success. Posted document [{i + 1} of {records}]. Record ID: {createdApDocument.ID}. CAAPS ID: {createdApDocument.CaapsRecordId}");
                            apDocument = createdApDocument;
                        }
                    }
                    else
                    {
                        logger.Error($"StatusCode: {response.StatusCode}. Error: Posting document [{i + 1} of {records}]: header id {apDocument.DocHeaderUID} not successful. Response: {RestSharpUtils.GetRestResponseErrorDetails(response)}");
                        failedOnPost++;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Failed to post [{i + 1} of {records}]: header id {apDocument.DocHeaderUID}: {e.Message}");
                    failedOnPost++;
                    continue;
                }

                if (apDocument != null && apDocument.ID > 0)
                {
                    try
                    {
                        string endpoint = string.Format(appSettings.UploadEndpoint, apDocument.ID);
                        Uri PostEndpointUrl = new Uri(endpoint);
                        logger.Debug($"Posting PDF [{i + 1} of {records}]: \"{endpoint}\"");

                        var utils2 = new RestSharpUtils(PostEndpointUrl, request2);
                        var response2 = await utils2.SendRequestAsync();
                        if (RestSharpUtils.IsResposeSuccess(response2))
                        {
                            ApDocumentResponse uploadedDocument = JsonConvert.DeserializeObject<ApDocumentResponse>(response2.Content);
                            if (uploadedDocument == null)
                            {
                                logger.Error($"Posted PDF [{i + 1} of {records}]: header id {apDocument.DocHeaderUID} not successful");
                                failedOnUpload++;
                            }
                            else
                            {
                                logger.Info($"Success. PDF Uploaded [{i + 1} of {records}]: FileID: {uploadedDocument.Id}. ServerFilePath: {uploadedDocument.ServerFilePath}");
                                success++;
                            }
                        }
                        else
                        {
                            logger.Error($"StatusCode: {response2.StatusCode}. Error: Uploading PDF document [{i + 1} of {records}]: header id {apDocument.DocHeaderUID} not successful. Response: {RestSharpUtils.GetRestResponseErrorDetails(response2)}");
                            failedOnPost++;
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, $"Failed to upload [{i + 1} of {records}]: header id {apDocument.DocHeaderUID}. {e.Message}");
                        failedOnUpload++;
                    }
                }
            }

            logger.Info($"Successfully posted records: {success} out of {records}");
            if (failedOnPost > 0 || failedOnUpload > 0)
            {
                exitCode = ExitCode.CompletedButWithErrors;

                logger.Error($"Failed on POST: {failedOnPost}");
                logger.Error($"Failed on PDF Upload: {failedOnUpload}");
            }
            return exitCode;
        }

        public ApDocument NewApDocument(int id)
        {
            var ApDocument = new ApDocument
            {
                DocHeaderUID = docHeader + id,
                DocDateIssued = DateTime.UtcNow,
                VendorCode = "TEST",
                DocRefNumberA = "TEST",
                DocAmountTotal = 0,
                EntityCode = "TEST",
                PoNumber = "TEST",
                DocAmountTax = 0,
                DocDescription = "TEST DESCRIPTION",
                DocDateReceived = DateTime.UtcNow,
                DocAmountTaxEx = 0,
                DocAmountCurrency = "AUD",
                DocReceivedSource = "TEST",
                DocReceivedType = "TEST",
                DocMultiplePoInHeaderYN = false,
            };
            ApDocument.LineItems.Add(
                new LineItem
                {
                    LineDescription = "TEST",
                    LineAmountTax = 0,
                    LineAmountTaxEx = 0,
                    LineAmountTotal = 0
                }
            );
            ApDocument.AccountCodingLines.Add(
                new GLCodeLine
                {
                    GLCode = "TEST",
                    GLCodeDesc = "TEST DESCRIPTION",
                    LineAmountTax = 0,
                    LineAmountTotal = 0,
                    LineAmountTaxEx = 0,
                    LineDescription = "TEST DESCRIPTION"
                }
            );
            return ApDocument;
        }
    }
}