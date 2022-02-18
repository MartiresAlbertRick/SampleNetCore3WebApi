using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NLog;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AD.CAAPS.Common
{
    public class RestSharpUtils
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Uri RequestUrl { get; }
        public RestRequest Request { get; }

        public RestSharpUtils(Uri requestUrl, RestRequest request)
        {
            RequestUrl = requestUrl ?? throw new ArgumentNullException(nameof(requestUrl));
            Request = request ?? throw new ArgumentNullException(nameof(request)); ;
        }

        private static bool RemoteCertificateValidate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert,
            System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            // trust any certificate!!!
#if TRACE_SSL_ERRORS
            if (cert is object)
            {
                logger.Trace(() => string.Format("SSL Certificate Subject: {0}. Issuer: {1}. Algo: {2}. Expiration: {3}", cert.Subject, cert.Issuer,
                    cert.GetKeyAlgorithmParametersString(),
                    cert.GetExpirationDateString()));
            }
            logger.Trace(() => "SSL Certificate validation disabled. Trusting any certificate.");
#endif
            return true;
        }


        public async Task<IRestResponse> DownloadToFilePath(string saveToFilePath)
        {
            var client = new RestClient(RequestUrl)
            {
                RemoteCertificateValidationCallback = RemoteCertificateValidate
            };
            return await DownloadToFilePath(client, saveToFilePath).ConfigureAwait(false);
        }

        public async Task<IRestResponse> DownloadToFilePath(IRestClient client, string saveToFilePath)
        {
            IRestResponse response;
            using (var writer = File.OpenWrite(saveToFilePath))
            {
                Request.ResponseWriter = responseStream =>
                {
                    using (responseStream)
                    {
                        responseStream.CopyToAsync(writer);
                    }
                };
                response = await SendRequestAsync(client).ConfigureAwait(false);
                if (response.IsSuccessful)
                {
                    await writer.FlushAsync().ConfigureAwait(false);
                }
            }
            if (!response.IsSuccessful)
            {
                await FileUtils.DeleteFile(saveToFilePath).ConfigureAwait(false);
            }
            return response;
        }

        public async Task<IRestResponse> SendRequestAsync()
        {
            var client = new RestClient(RequestUrl)
            {
                RemoteCertificateValidationCallback = RemoteCertificateValidate
            };
            return await SendRequestAsync(client).ConfigureAwait(false);
        }

        public async Task<IRestResponse> SendRequestAsync(IRestClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (Request == null) throw new NullReferenceException($"Referenced unassigned {nameof(Request)} object.");
            logger.Info($"HTTP Request {Request.Method} {client.BaseUrl} {Request.Resource}");
            IRestResponse response = await client.ExecuteAsync(Request, Request.Method).ConfigureAwait(false);
            string prefix = $"HTTP Request error StatusCode: {(int)response.StatusCode}.";
            logger.Debug($"HTTP Request completed {Request.Method}: StatusCode: {(int)response.StatusCode}. StatusDescription: \"{response.StatusDescription}\"", response);
            if (!response.IsSuccessful && !string.IsNullOrWhiteSpace(response.StatusDescription)) logger.Error($"{prefix} {response.StatusDescription}");
            if (!string.IsNullOrWhiteSpace(response.ErrorMessage)) logger.Error($"{prefix} Error message: {response.ErrorMessage}");
            if (response.ErrorException != null) logger.Error(response.ErrorException, $"{prefix} Exception message: {response.ErrorException.Message}");
            if (response.ErrorException != null) logger.Error(response.ErrorException, $"{prefix} Exception details: {response.ErrorException}");
            if (response.Content != null && response.Content.Length > 0)
            {
                if (!response.IsSuccessful)
                {
                    logger.Error(() => $"{prefix} Content {response.ContentType}: \r\n{response.Content}\r\n");
                    if (response.ContentType.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase))
                    {
                        JToken content = JToken.Parse(response.Content);
                        if (content != null && content.Type == JTokenType.String)
                        {
                            logger.Error(() => $"{prefix} Content JSON: \r\n{content}\r\n");
                        }
                    }
                }
                else
                {
                    logger.Trace(() => $"HTTP Request response StatusCode: {(int)response.StatusCode}. Content: {response.ContentType} \r\n{response.Content}\r\n");
                }
            }
            return response;
        }

        public static string GetRestResponseErrorDetails(IRestResponse response)
        {
            string details;
            if (response != null)
            {
                details = $"Code: {(int)response.StatusCode}. Status: {response.StatusDescription}.";
                if (!string.IsNullOrWhiteSpace(response.ErrorMessage)) details += $" Error: {response.ErrorMessage}.";
                if (response.ErrorException != null) details += $" Exception: {response.ErrorException.Message}.";
                if (!string.IsNullOrWhiteSpace(response.Content)) details += $" Content: {response.Content}.";
                return details;
            }
            else
                return "Response undefined";
        }

        public static bool IsResposeSuccess(IRestResponse response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response), "The response paramater must be assigned");
            }
            logger.Trace(() => $"StatusCode: {(int)response.StatusCode}. Status: {response.StatusDescription}. IsSuccessful: {response.IsSuccessful}. Content-Type: {response.ContentType}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static RestClient ClientWithBasicAuthentication(Uri requestUrl, string username, string password)
        {
            return new RestClient(requestUrl)
            {
                RemoteCertificateValidationCallback = RemoteCertificateValidate,
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
        }

        public static RestRequest CreateRequestForPostJson()
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            return request;
        }

        public static RestRequest CreateRequestForPut()
        {
            var request = new RestRequest(Method.PUT);
            return request;
        }

        public static RestRequest CreateRequestForDownloadPDFFromADAzurePortal(string xClientId, string ocpApimTrace, string ocpApimSubscriptionKey)
        {
            var request = new RestRequest(Method.GET);

            request.AddHeader("X-Client-Id", xClientId);
            request.AddHeader("Ocp-Apim-Trace", ocpApimTrace);
            request.AddHeader("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey);

            return request;
        }

        public static RestRequest CreateRequestForUploadPDF(string fileName)
        {
            logger.Trace($"Creating 'application/pdf' POST request for file {fileName}");
            var request = new RestRequest(Method.POST);
            request.AddFile("file", fileName, "application/pdf");
            return request;
        }
    }
}