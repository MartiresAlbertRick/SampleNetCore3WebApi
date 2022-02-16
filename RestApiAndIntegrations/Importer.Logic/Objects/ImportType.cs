using AD.CAAPS.Importer.Common;
using System.Collections.Generic;

namespace AD.CAAPS.Importer.Logic
{
    public class ImportType
    {
        public string DefaultImportFilePath { get; set; }
        public string DefaultImportFileName { get; set; }
        public bool FileNameIsRegex { get; set; }
        public bool MatchCurrentDateAndFileCreationDate { get; set; }
        public bool MatchCurrentDateAndFileNameDate { get; set; }
        public string FileNameDateTimePattern { get; set; }
        public bool GetTopOneFileByCreationDate { get; set; }
        public ActionAfterImport ActionAfterImport { get; set; }
        public string TargetFolderAfterImport { get; set; }
        public string Route { get; set; }
        public string Culture { get; set; } = "en-AU";
        public ClearTargetTableSetting ClearTargetTableSetting { get; set; }
        public Dictionary<string, string> CaapsApiModelDbFieldsMapping { get; } = new Dictionary<string, string>();
        public string FieldDelimiter { get; set; } = ",";
    }
}
