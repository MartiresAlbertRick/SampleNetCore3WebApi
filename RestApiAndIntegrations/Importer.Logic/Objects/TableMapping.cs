using System.Xml.Serialization;

namespace AD.CAAPS.Importer.Logic
{
    public class TableMapping
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("SourceFileName")]
        public string SourceFileName { get; set; }
        [XmlAttribute("FieldDelimiter")]
        public string FieldDelimiter { get; set; }
        [XmlAttribute("SourceFileLocale")]
        public string SourceFileLocale { get; set; }
        [XmlAttribute("TargetTableName")]
        public string TargetTableName { get; set; }
        [XmlAttribute("ClearTargetTable")]
        public string ClearTargetTable { get; set; }
        [XmlAttribute("UseSourceFileNamePattern")]
        public string UseSourceFileNamePattern { get; set; }
        [XmlAttribute("SourceFileNamePattern")]
        public string SourceFileNamePattern { get; set; }
        [XmlAttribute("ProcessNewestSourceFile")]
        public string ProcessNewestSourceFile { get; set; }

        [XmlElement("FieldMappings")]
        public FieldMappings FieldMappings { get; set; }
    }
}