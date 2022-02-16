using System.Xml.Serialization;

namespace AD.CAAPS.Importer.Logic
{
    public class FieldMapping
    {
        [XmlAttribute("SourceFieldName")]
        public string SourceFieldName { get; set; }
        [XmlAttribute("TargetFieldName")]
        public string TargetFieldName { get; set; }
        [XmlAttribute("TrimSpaces")]
        public string TrimSpaces { get; set; }
    }
}