using System.Collections.Generic;

namespace AD.CAAPS.Importer.Urbanise
{
    public class AppSettings
    {
        public bool SaveClientDataToFile { get; set; }
        public string ClientDataFilePath { get; set; }
        public bool AllowPagination { get; set; }
        public int RecordLimit { get; set; }
        public int ClientSupplierPageLimit { get; set; }
        public int ClientPropertyPageLimit { get; set; }
        public string PageQuery { get; set; }
        public string UrbanisePropertyEndpoint { get; set; }
        public string UrbaniseSupplierEndpoint { get; set; }
        public string ClientEmailSignature { get; set; }
        public string ClientBearerToken { get; set; }
        public bool ClientSupportsCompression { get; set; }
#pragma warning disable CA1056 // Uri properties should not be strings
        public string CAAPSApiBaseUrl { get; set; }
        public string TruncateTableUrlParam { get; set; }
        public string CAAPSEntityResourceUri { get; set; }
        public string CAAPSVendorResourceUri { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings
        public int PostPageSizeLimit { get; set; }
        public bool CAAPSSupportsCompression { get; set; }
        public string XClientId { get; set; }
        public string OcpApimTrace { get; set; }
        public string OcpApimSubscriptionKey { get; set; }
        public List<Dictionary<string, string>> Clients { get; } = new List<Dictionary<string, string>>();
    }
}