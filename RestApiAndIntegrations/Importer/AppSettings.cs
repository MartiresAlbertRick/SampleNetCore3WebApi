using AD.CAAPS.Importer.Logic;
using System.Collections.Generic;

namespace AD.CAAPS.Importer
{
    public class EmailNotifications
    {
        public string Sender { get; set; }
        public string RecipientList { get; set; }
        public string CCList { get; set; }
        public string BCCList { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "All appsettings.json configuration parameters are strings and optional. Uri validation is performed when required.")]
    public class AppSettings : IImportBuilder
    {
        public string SendGridAPIKey { get; set; }
        public EmailNotifications EmailNotifications { get; set; }
        public System.Uri ApiUrl { get; set; }
        public string TruncateTableParameter { get; set; }
        public int PostPageSizeLimit { get; set; }
        public ClientSettings ClientSettings { get; set; }
        public Dictionary<string, string> HttpRequestHeaders { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> HttpRequestParameters { get; } = new Dictionary<string, string>();
        public Dictionary<string, ImportType> ImportTypes { get; } = new Dictionary<string, ImportType>();
    }
}