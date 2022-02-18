using System;
using System.Collections.Generic;

namespace AD.CAAPS.Importer.Logic
{
    public class ImportBuilder : IImportBuilder
    {
        public Uri ApiUrl { get; set; }
        public int PostPageSizeLimit { get; set; }
        public string TruncateTableParameter { get; set; }
        public Dictionary<string, string> HttpRequestHeaders { get; }
        public Dictionary<string, string> HttpRequestParameters { get; }
        public Dictionary<string, ImportType> ImportTypes { get; }
        public Uri CAAPSApiBaseUrl { get; set; }
        public ClientSettings ClientSettings { get; set; }

        public ImportBuilder()
        {
            HttpRequestHeaders = new Dictionary<string, string>();
            HttpRequestParameters = new Dictionary<string, string>();
            ImportTypes = new Dictionary<string, ImportType>();
        }
    }
}