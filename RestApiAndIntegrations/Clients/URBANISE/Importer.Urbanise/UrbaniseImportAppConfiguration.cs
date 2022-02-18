using AD.CAAPS.Importer.Common;
using System.Collections.Generic;

namespace AD.CAAPS.Importer.Urbanise
{
    internal class UrbaniseImportAppConfiguration : IImportBuilderBase
    {
        public System.Uri ApiUrl { get; set; }
        public int PostPageSizeLimit { get; set; }
        public string TruncateTableParameter { get; set; }
        public Dictionary<string, string> HttpRequestHeaders { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> HttpRequestParameters { get; } = new Dictionary<string, string>();
        public UrbaniseImportAppConfiguration(System.Uri CAAPSApiRoot, int PostPageSizeLimit, string TruncateTableUrlParam)
        {
            this.ApiUrl = CAAPSApiRoot;
            this.PostPageSizeLimit = PostPageSizeLimit;
            this.TruncateTableParameter = TruncateTableUrlParam;
        }
    }
}