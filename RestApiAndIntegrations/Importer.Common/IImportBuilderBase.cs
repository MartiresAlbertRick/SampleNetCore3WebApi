using System.Collections.Generic;

namespace AD.CAAPS.Importer.Common
{
    public interface IImportBuilderBase
    {
        System.Uri ApiUrl { get; set; }
        int PostPageSizeLimit { get; set; }
        string TruncateTableParameter { get; set; }
        Dictionary<string, string> HttpRequestHeaders { get; }
        Dictionary<string, string> HttpRequestParameters { get; }
    }
}