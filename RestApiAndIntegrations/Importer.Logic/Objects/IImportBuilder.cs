using AD.CAAPS.Importer.Common;
using System.Collections.Generic;

namespace AD.CAAPS.Importer.Logic
{
    public interface IImportBuilder : IImportBuilderBase
    {
        ClientSettings ClientSettings { get; set; }
        Dictionary<string, ImportType> ImportTypes { get; }
    }

    public class ClientSettings {
        public string ClientId { get; set; }
        public bool AddClientIdAsHttpHeader { get; set; }
        public string ClientIdHttpHeaderName { get; set; }
    }
}