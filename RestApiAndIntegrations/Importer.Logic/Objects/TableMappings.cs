using System.Collections.Generic;
using System.Xml.Serialization;

namespace AD.CAAPS.Importer.Logic
{
    [XmlRoot("TableMappings")]
    public class TableMappings
    {
        public TableMappings()
        {
            Items = new List<TableMapping>();
        }

        [XmlElement("TableMapping")]
        public List<TableMapping> Items { get; }
    }
}