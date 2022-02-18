using System.Collections.Generic;
using System.Xml.Serialization;

namespace AD.CAAPS.Importer.Logic
{
    public class FieldMappings
    {
        public FieldMappings()
        {
            Items = new List<FieldMapping>();
        }

        [XmlElement("FieldMapping")]
        public List<FieldMapping> Items { get; }
    }
}