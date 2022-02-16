using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Importer.Common;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AD.CAAPS.Importer.Urbanise
{
    public class APIImporter
    {
        private readonly AppSettings appSettings;
        private readonly ImportObjectType objectType;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public APIImporter(AppSettings appSettings, ImportObjectType objectType)
        {
            if (objectType != ImportObjectType.Entity && objectType != ImportObjectType.Vendor)
                throw new NotImplementedException($"No implementation for object type {objectType}. Supported object types are: 1 - Vendor (ERP Suppliers) and 6 - Entity (ERP Properties)");
            this.appSettings = appSettings ?? throw new NullReferenceException($"Configuration missing. The object \"{nameof(appSettings)}\" is null.");
            this.objectType = objectType;
        }


        private async Task GetClientDataBatches<T>(Uri clientUrbaniseEndpoint, List<T> data)
        {
            string urbaniseResponseBody;
            int totalCharactersReceived = 0;
            int page = 0;
            int RecordLimit = 0;
            if (appSettings.RecordLimit > 0)
            {
                RecordLimit = appSettings.RecordLimit;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (!appSettings.AllowPagination)
            {
                logger.Debug($"Starting {objectType} full request");
                urbaniseResponseBody = await SendUrbaniseRequest(clientUrbaniseEndpoint, appSettings.ClientBearerToken);
                totalCharactersReceived += urbaniseResponseBody.Length;
                data = JsonConvert.DeserializeObject<List<T>>(urbaniseResponseBody);
                if (appSettings.SaveClientDataToFile)
                {
                    await FileUtils.SaveJSONToFile(Path.Combine(appSettings.ClientDataFilePath, $"ERP-{objectType}-full.json"), urbaniseResponseBody);
                }
            }
            else
            {
                int PageLimit = objectType == ImportObjectType.Vendor ? appSettings.ClientSupplierPageLimit : appSettings.ClientPropertyPageLimit;
                string pageQuery = appSettings.PageQuery;
                logger.Debug($"Starting {objectType} batch request using paged query: {pageQuery}");
                bool proceed = true;
                do
                {
                    page++;

                    // would display something like this ?page=1&size=100
                    string pageQueryFormatted = string.Format(pageQuery, page, PageLimit);
                    Uri combinedUrl = new Uri(clientUrbaniseEndpoint.ToString() + pageQueryFormatted);

                    Stopwatch pageStopwatch = Stopwatch.StartNew();
                    urbaniseResponseBody = await SendUrbaniseRequest(combinedUrl, appSettings.ClientBearerToken);
                    pageStopwatch.Stop();
                    logger.Trace($"Time elapsed for this request {pageStopwatch.ElapsedMilliseconds:N0}ms");
                    totalCharactersReceived += urbaniseResponseBody.Length;
                    pageStopwatch.Restart();
                    List<T> deserialized = JsonConvert.DeserializeObject<List<T>>(urbaniseResponseBody);
                    logger.Trace($"Time elapsed for parsing request {pageStopwatch.ElapsedMilliseconds:N0}ms");
                    logger.Debug($"Got {deserialized.Count:N0} {objectType} records in this batch + previous records: {data.Count:N0}. Total characters retrieved: {totalCharactersReceived:N0}");
                    if (deserialized.Count > 0)
                    {
                        if (appSettings.SaveClientDataToFile)
                        {
                            await FileUtils.SaveJSONToFile(Path.Combine(appSettings.ClientDataFilePath, $"ERP-{objectType}-page-{page}.json"), urbaniseResponseBody);
                        }
                        data.AddRange(deserialized);
                        if (RecordLimit > 0 && data.Count >= RecordLimit)
                        {
                            logger.Trace($"Total record limit {RecordLimit:N0} reached - skipping remaining {objectType} records");
                            proceed = false;
                        }
                        if (deserialized.Count < PageLimit)
                        {
                            logger.Trace($"Records returned count less than page limit {PageLimit:N0} - skipping remaining {objectType} records");
                            proceed = false;
                        }
                    }
                    else
                    {
                        proceed = false;
                    }
                }
                while (proceed);
            }
            stopwatch.Stop();
            logger.Debug($"No more {objectType} data to retrieve on page {page:N0}. Time for all requests {stopwatch.ElapsedMilliseconds:N0}ms");
            logger.Info($"Total {objectType} records: {data.Count:N0}. Total characters retrieved: {totalCharactersReceived:N0}");

        }

        public async Task<ExitCode> Import<T>()
        {
            logger.Debug($"{GetType().Name} Import Process starting");
            Stopwatch processStopwatch = Stopwatch.StartNew();

            Uri urbaniseEndpoint;
            Uri caapsEndpoint;

            if (string.IsNullOrWhiteSpace(appSettings.UrbaniseSupplierEndpoint))
                throw new ConfigurationMissingException(nameof(appSettings.UrbaniseSupplierEndpoint), "Check configuration. ERP API Endpoint not configured.");
            if (string.IsNullOrWhiteSpace(appSettings.UrbanisePropertyEndpoint))
                throw new ConfigurationMissingException(nameof(appSettings.UrbanisePropertyEndpoint), "Check configuration. ERP API Endpoint not configured.");
            if (string.IsNullOrWhiteSpace(appSettings.CAAPSApiBaseUrl))
                throw new ConfigurationMissingException(nameof(appSettings.CAAPSApiBaseUrl), "Check configuration. CaapsApiBaseUrl Endpoint not configured.");

            logger.Debug($"{GetType().Name} UrbaniseSupplierEndpoint: {appSettings.UrbaniseSupplierEndpoint}");
            logger.Debug($"{GetType().Name} UrbanisePropertyEndpoint: {appSettings.UrbanisePropertyEndpoint}");
            logger.Debug($"{GetType().Name} CAAPSApiBaseUrl: {appSettings.CAAPSApiBaseUrl}");
            logger.Debug($"{GetType().Name} CAAPSVendorResourceUri: {appSettings.CAAPSVendorResourceUri}");
            logger.Debug($"{GetType().Name} CAAPSEntityResourceUri: {appSettings.CAAPSEntityResourceUri}");

            switch (objectType)
            {
                case (ImportObjectType.Vendor):
                    urbaniseEndpoint = new Uri(appSettings.UrbaniseSupplierEndpoint);
                    caapsEndpoint = new Uri(Utils.EnsureTrailingUrlSeparator(new System.Uri(appSettings.CAAPSApiBaseUrl)) + appSettings.CAAPSVendorResourceUri);
                    break;
                case (ImportObjectType.Entity):
                    urbaniseEndpoint = new Uri(appSettings.UrbanisePropertyEndpoint);
                    caapsEndpoint = new Uri(Utils.EnsureTrailingUrlSeparator(new System.Uri(appSettings.CAAPSApiBaseUrl)) + appSettings.CAAPSEntityResourceUri);
                    break;
                default:
                    // return Utils.SetEnvironmentExitCode((int)ExitCode.ErrorOnProcessStart);
                    throw new NotImplementedException($"Object type not defined. Value is {objectType}");
            }

            int clientProcessed = 0, clientSuccess = 0, clientFailed = 0;
            foreach (var client in appSettings.Clients)
            {
                clientProcessed++;
                try
                {
                    string clientId = client["ClientId"];
                    string organisation = client["Organisation"];
                    string DefaultProcessingCurrency = client["DefaultProcessingCurrency"];
                    string country = client["Country"];
                    if (string.IsNullOrWhiteSpace(clientId))
                        throw new NullReferenceException($"Check configuration. Missing values for client configuration {nameof(clientId)}.");
                    if (string.IsNullOrWhiteSpace(organisation))
                        throw new NullReferenceException($"Check configuration. Missing values for client configuration {nameof(organisation)}.");
                    if (string.IsNullOrWhiteSpace(country))
                        throw new NullReferenceException($"Check configuration. Missing values for client configuration {nameof(country)}.");
                    if (string.IsNullOrWhiteSpace(DefaultProcessingCurrency))
                        throw new NullReferenceException($"Check configuration. Missing values for client configuration {nameof(DefaultProcessingCurrency)}.");

                    string clientUrbaniseEndpoint = string.Format(urbaniseEndpoint.ToString(), clientId);
                    string clientCaapsEndpoint = string.Format(caapsEndpoint.ToString(), clientId);

                    var clientUrbaniseEndpointUri = new Uri(clientUrbaniseEndpoint);
                    logger.Debug($"Sending ERP request to host: {clientUrbaniseEndpointUri.Scheme}://{clientUrbaniseEndpointUri.Host}:{clientUrbaniseEndpointUri.Port} - {clientUrbaniseEndpointUri.AbsoluteUri}");

                    var clientCaapsEndpointUri = new Uri(clientCaapsEndpoint);
                    logger.Debug($"Sending CAAPS request to host: {clientCaapsEndpointUri.Scheme}://{clientCaapsEndpointUri.Host}:{clientCaapsEndpointUri.Port} - {clientCaapsEndpointUri.AbsoluteUri}");

                    var data = new List<T>();

                    await GetClientDataBatches(clientUrbaniseEndpointUri, data);

                    if (data.Count == 0)
                        throw new NullReferenceException("No records to process. Data is required.");

                    if (appSettings.SaveClientDataToFile)
                    {
                        string jsonSerialized = JsonConvert.SerializeObject(data, Formatting.Indented);
                        await FileUtils.SaveJSONToFile(Path.Combine(appSettings.ClientDataFilePath, $"ERP-{objectType}-raw-{DateTime.Now:yyyy'-'MM'-'dd}.json"), jsonSerialized);
                    }

                    switch (objectType)
                    {
                        case ImportObjectType.Vendor:
                            IList<Vendor> vendors = ConvertSupplierToVendor(data as List<Supplier>, clientId);
                            await SendCaapsRequest<Vendor>(clientCaapsEndpointUri, vendors);
                            break;
                        case ImportObjectType.Entity:
                            IList<Entity> entities = ConvertPropertyToEntity(data as List<PropertyPlan>, clientId, organisation, country,
                                DefaultProcessingCurrency);
                            await SendCaapsRequest<Entity>(clientCaapsEndpointUri, entities);
                            break;
                        default:
                            throw new NotImplementedException("Processing for this entity type is not supported.");
                    }

                    clientSuccess++;
                    logger.Trace($"Upload of {objectType} to \"{clientCaapsEndpoint}\" success for client \"{clientId}\"");
                }
                catch (Exception e)
                {
                    clientFailed++;
                    logger.Error(e, e.Message);
                }
            }

            processStopwatch.Stop();
            logger.Debug($"Process completed. Success: {clientSuccess:N0} / {clientProcessed:N0}. Failed: {clientFailed:N0}. Time for all requests {processStopwatch.ElapsedMilliseconds:N0}ms");

            if (clientFailed > 0)
                return ExitCode.CompletedButWithErrors;

            return ExitCode.Successful;
        }

        private static IList<Vendor> ConvertSupplierToVendor(IList<Supplier> suppliers, string entityCode)
        {
            var dataset = new List<Vendor>(suppliers.Count);
            foreach (var supplier in suppliers)
            {
                dataset.Add(
                    new Vendor
                    {
                        VendorCode = supplier.SupplierId,
                        VendorName = supplier.SupplierName,
                        BankBsbNumber = supplier.AccountBsb,
                        BankAccountNumber = supplier.AccountNumber,
                        VendorAddressLine01 = NormalizeLength(supplier.Address, 0, 100),
                        VendorCity = NormalizeLength(supplier.City, 0, 50),
                        VendorCountry = NormalizeLength(supplier.Country, 0, 50),
                        VendorPostCode = supplier.Postcode,
                        VendorState = NormalizeLength(supplier.State, 0, 50),
                        VendorBusinessNumber = supplier.SupplierABN,
                        PaymentTypeCode = supplier.UseEFT ? "EFT" : null,
                        BPAYBillerCode = supplier.BpayBillerCode,
                        Custom01 = supplier.BpayBillerCode,
                        Custom02 = supplier.AccountName,
                        Custom03 = supplier.UseEFT ? "EFT" : null,
                        Custom04 = supplier.GstRegistrationNumber,
                        EntityCode = entityCode
                    }
               );
            }
            return dataset;
        }

        private IList<Entity> ConvertPropertyToEntity(IList<PropertyPlan> properties, string entityCode, string entityName, string country, string defaultProcessingCurrency)
        {
            var dataset = new List<Entity>(properties.Count);
            string ClientEmailSignature = appSettings.ClientEmailSignature;
            foreach (var property in properties)
            {
                string address = "";
                switch (country)
                {
                    case "AU":
                        address = FormatAddress(
                                    property.PlanStAddress,
                                    property.PlanSuburb,
                                    property.PlanState,
                                    property.PlanPostCode,
                                    property.PlanCountry);
                        break;
                    case "NZ":
                        address = FormatAddress(
                                    property.PlanStAddress,
                                    property.PlanSuburb,
                                    property.PlanState,
                                    property.PlanPostCode,
                                    property.PlanRegion,
                                    property.PlanCountry);
                        break;
                }

                dataset.Add(
                    new Entity
                    {
                        EntityCode = entityCode,
                        EntityName = entityName,
                        EntityBusinessNumber = property.PlanABN,
                        ReferenceAddress = address,
                        SiteCode = property.PlanNumber,
                        SiteName = property.PlanName,
                        ProcessingCurrency = defaultProcessingCurrency,
                        EmailSignOff = ClientEmailSignature,
                        Custom01 = property.PlanGST,
                    });
            }

            return dataset;
        }

        private async Task<string> SendUrbaniseRequest(Uri requestUrl, string BearerToken)
        {
            var getDataRequest = new RestRequest(Method.GET);
            logger.Info($"Request {Method.GET} \"{requestUrl}\" starting");
            if (!string.IsNullOrWhiteSpace(BearerToken))
            {
                getDataRequest.AddHeader("Authorization", "Bearer " + BearerToken);
            }
            if (appSettings.ClientSupportsCompression)
            {
                logger.Trace("Requesting response compression");
                getDataRequest.AddHeader("Accept-Encoding", "gzip");
            }
            var getDataUtils = new RestSharpUtils(requestUrl, getDataRequest);
            Stopwatch stopwatch = Stopwatch.StartNew();
            IRestResponse response = await getDataUtils.SendRequestAsync();
            stopwatch.Stop();
            logger.Trace(() => $"Request completed with status code {response.StatusCode} in {stopwatch.ElapsedMilliseconds:N0} ms");
            if (!response.IsSuccessful)
            {
                logger.Error($"Request failed \"{requestUrl}\" with StatusCode {response.StatusCode} {response.StatusDescription} in {stopwatch.ElapsedMilliseconds:N0}ms");
                string message = RestSharpUtils.GetRestResponseErrorDetails(response);
                throw new HttpRequestException(message);
            }
            if (string.IsNullOrWhiteSpace(response.Content))
            {
                logger.Error($"Request failed \"{requestUrl}\" with StatusCode {response.StatusCode} {response.StatusDescription} (empty body) in {stopwatch.ElapsedMilliseconds:N0}ms");
                string message = RestSharpUtils.GetRestResponseErrorDetails(response);
                throw new NullReferenceException(message);
            }
            logger.Trace(() => $"Response Content Type: {response.ContentType}. ContentEncoding: {response.ContentEncoding}. Content.Length: {response.Content.Length:N0}");
            for (int index = 0; index < response.Headers.Count; index++)
            {
                logger.Trace($"Response Header[{index}]: \"{response.Headers[index].Name}\": \"{response.Headers[index].Value}\"");
            }
            return response.Content;
        }

        private async Task SendCaapsRequest<T>(Uri requestUrl, IList<T> dataPayload) where T : class
        {
            if (appSettings.SaveClientDataToFile)
            {
                string jsonSerialized = JsonConvert.SerializeObject(dataPayload, Formatting.Indented);
                await FileUtils.SaveJSONToFile(Path.Combine(appSettings.ClientDataFilePath, $"ERP-{objectType}-remapped-{DateTime.Now:yyyy'-'MM'-'dd}.json"), jsonSerialized);
            }
            var ImportAppConfiguration = new UrbaniseImportAppConfiguration(requestUrl, appSettings.PostPageSizeLimit, appSettings.TruncateTableUrlParam);

            ImportAppConfiguration.HttpRequestHeaders.Add("X-Client-Id", appSettings.XClientId);
            if (!string.IsNullOrWhiteSpace(appSettings.OcpApimTrace))
            {
                ImportAppConfiguration.HttpRequestHeaders.Add("Ocp-Api-Trace", appSettings.OcpApimTrace);
            }
            if (!string.IsNullOrWhiteSpace(appSettings.OcpApimSubscriptionKey))
            {
                ImportAppConfiguration.HttpRequestHeaders.Add("Ocp-Apim-Subscription-Key", appSettings.OcpApimSubscriptionKey);
            }

            var bulkImporter = new CAAPSApiBulkDataImport<T>();
            bulkImporter.ConfigureBulkImport(ImportAppConfiguration);
            await bulkImporter.Start(dataPayload, ClearTargetTableSetting.Truncate);
        }

        private static string NormalizeLength(string value, int startIndex, int length)
        {
            return value.Length > length ? value.Substring(startIndex, length) : value;
        }

        private static string NormalizeSpaces(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";
            else
            {
                value = Regex.Replace(value, @"\s{2,}", " ");
                return value;
            }
        }

        private static string FormatAddress(params string[] args)
        {
            string address = "";

            foreach (string arg in args)
            {
                if (!string.IsNullOrWhiteSpace(arg))
                    address += NormalizeSpaces(arg) + " ";
            }

            return NormalizeSpaces(address.Trim());
        }
    }
}