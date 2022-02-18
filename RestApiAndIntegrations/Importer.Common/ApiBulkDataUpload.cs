using AD.CAAPS.Common;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AD.CAAPS.Importer.Common
{
    public class CAAPSApiBulkDataImport<T>
    {
        const string ContentTypeKey = "content-type";
        const string ContentTypeValue = "application/json";

        protected static IImportBuilderBase Configuration { get; private set; }

        public static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void ConfigureBulkImport(IImportBuilderBase configuration)
        {
            CAAPSApiBulkDataImport<T>.Configuration = configuration;
        }

        public async Task Start(IList<T> data, ClearTargetTableSetting clearTargetTableSetting)
        {
            //caaps api import
            if (Configuration.PostPageSizeLimit > 0)
            {
                await SendPagedRequestCaapsApiBulkDataImport(data, clearTargetTableSetting);
            }
            else
            {
                await SendRequestCaapsApiBulkDataImport(data, true, clearTargetTableSetting);
            }
        }

        public async Task SendPagedRequestCaapsApiBulkDataImport(IList<T> data, ClearTargetTableSetting clearTargetTableSetting)
        {
            IList<T> PagedData = new List<T>();
            int Imported = 0;
            bool FirstPage = true;
            int PageSizeLimit = Configuration.PostPageSizeLimit;
            while (Imported < data.Count)
            {
                PagedData.Add(data[Imported]);
                Imported++;
                if (PagedData.Count == PageSizeLimit)
                {
                    logger.Trace($"Sending paged data {PagedData.Count:N0} records");
                    await SendRequestCaapsApiBulkDataImport(PagedData, FirstPage, clearTargetTableSetting);
                    PagedData.Clear();
                    FirstPage = false;
                }
            }
            if (PagedData.Count > 0)
            {
                await SendRequestCaapsApiBulkDataImport(PagedData, FirstPage, clearTargetTableSetting);
            }
        }

        private Uri BuildBulkPagedResourceUrl(bool firstPage, ClearTargetTableSetting clearTargetTableSetting)
        {
            Utils.CheckObjectIsNullThrowException(Configuration.ApiUrl, () => new ArgumentNullException(nameof(Configuration.ApiUrl), "No value set for CAAPS Api Root Url"));
            if (!firstPage) return Configuration.ApiUrl;

            string truncateQuery = Configuration.TruncateTableParameter;
            if (string.IsNullOrWhiteSpace(truncateQuery))
            {
                truncateQuery = "?truncateTable=";
            }
            
            if (Configuration.ApiUrl.ToString().Contains("?"))
            {
                truncateQuery = truncateQuery.Replace('?', '&');
            }

            logger.Trace($"Config ClearTable setting: \"{clearTargetTableSetting}\"");
            Uri ApiUrl;
            if (clearTargetTableSetting == ClearTargetTableSetting.Truncate || clearTargetTableSetting == ClearTargetTableSetting.Delete)
            {
                ApiUrl = new Uri(Configuration.ApiUrl + truncateQuery + "true");
            }
            else
            {
                ApiUrl = new Uri(Configuration.ApiUrl + truncateQuery + "false");
            }
            logger.Debug($"CAAPS API target URL: \"{ApiUrl}\"");
            return ApiUrl;
        }

        public async Task SendRequestCaapsApiBulkDataImport(IList<T> data, bool firstPage, ClearTargetTableSetting clearTargetTableSetting)
        {
            Utils.CheckListLengthIsZeroThrowException(data, () => new ArgumentOutOfRangeException(nameof(data), "The data parameter must not be an empty list."));
            string json = JsonConvert.SerializeObject(data);
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(json, () => new InvalidDataException("The JSON payload must be a valid JSON string when POSTing data to the CAAPS API"));
            Uri ApiUrl = BuildBulkPagedResourceUrl(firstPage, clearTargetTableSetting);
            var method = RestSharp.Method.POST;
            var request = new RestSharp.RestRequest(method);
            logger.Trace($"Sending {method} {ApiUrl} Records: {data.Count:N0} as {ContentTypeValue}");
            //Add headers in request
            request.AddHeader(ContentTypeKey, ContentTypeValue);
            foreach (var key in Configuration.HttpRequestHeaders.Keys)
            {
                request.AddHeader(key, Configuration.HttpRequestHeaders[key]);
            }
            logger.Trace(() => $"POST \"{ContentTypeValue}\" data with {json.Length:N0} characters");
            //Add parameter
            request.AddParameter(ContentTypeValue, json, RestSharp.ParameterType.RequestBody);

            var utils = new RestSharpUtils(ApiUrl, request);
            RestSharp.IRestResponse response = await utils.SendRequestAsync();
            if (!response.IsSuccessful)
            {
                throw new Exception($"CAAPS API failed to send request to \"{ApiUrl}\" and resulted in {response.StatusCode}");
            }
            logger.Info($"CAAPS API Response for \"{ApiUrl}\" is: \r\n{response.Content}\r\n");
        }
    }
}
