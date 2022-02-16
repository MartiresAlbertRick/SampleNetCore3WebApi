using AD.CAAPS.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AD.CAAPS.Importer.Logic
{
    public class DvDataImportConfiguration
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string configurationFilePath;
        public TableMappings TableMappings { get; set; }
        
        public DvDataImportConfiguration(string configurationFilePath)
        {
            logger.Debug($"Initializing DataImportConfigurationHelper with parameter: {nameof(configurationFilePath)} value: {configurationFilePath}");
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(configurationFilePath, () => new ArgumentException($"No value assigned for {nameof(configurationFilePath)}"));
            this.configurationFilePath = configurationFilePath;
        }

        public TableMapping GetTableMappingByName(string tableMappingName)
        {
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(tableMappingName, () => new ArgumentException($"No value assigned for {nameof(tableMappingName)}"));
            TableMappings.Items.ForEach(e => logger.Trace(() => $"Existing table mappings: \"{e.Name}\" \"{e.TargetTableName}\" \"{e.SourceFileName}\""));
            TableMapping tableMapping = TableMappings.Items
                                .Where(t => t.Name.Equals(tableMappingName, StringComparison.OrdinalIgnoreCase))
                                .SingleOrDefault();
            return tableMapping ?? throw new KeyNotFoundException($"Unable to locate key of {nameof(tableMappingName)} from configuration for \"{tableMappingName}\".");
        }

        public Dictionary<string, string> GetFieldMappingsAsDictionary(string tableMappingName)
        {
            TableMapping tableMapping = GetTableMappingByName(tableMappingName);
            var fieldMappings = new Dictionary<string, string>();
            foreach (FieldMapping fieldMapping in tableMapping.FieldMappings.Items)
            {
                fieldMappings.Add(fieldMapping.TargetFieldName, fieldMapping.SourceFieldName);
            }
            return fieldMappings;
        }

        public void BuildTableMappings()
        {
            logger.Debug($"Starting to build table mappings from xml configuration file \"{configurationFilePath}\"");
            using var stringReader = new StringReader(XmlString());
            using var xmlTextReader = new XmlTextReader(stringReader);
            var xmlSerializer = new XmlSerializer(typeof(TableMappings));
            TableMappings = (TableMappings)xmlSerializer.Deserialize(xmlTextReader);
        }

        private string XmlString()
        {
            logger.Debug($"Attempting to read xml configuration file \"{configurationFilePath}\"");
            using XmlReader reader = XmlReader.Create(configurationFilePath);
            Utils.CheckObjectIsNullThrowException(reader, () => new FileLoadException($"Nothing to read from xml configuration file \"{configurationFilePath}\""));
            StringBuilder builder = new StringBuilder();
            while (reader.ReadToFollowing("TableMappings"))
            {
                builder.AppendLine(reader.ReadOuterXml());
            }
            string builtString = builder.ToString();
            logger.Trace($"Retrieved configuration: {builtString}");
            return builtString;
        }
    }
}
