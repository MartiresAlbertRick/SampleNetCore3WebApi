using AD.CAAPS.Common;
using System;
using System.Collections.Generic;

namespace AD.CAAPS.Importer.Logic
{
    public class BaseReader
    {
        const string DefaultCulture = "en-AU";
        const string DefaultDelimiter = ",";

        public BaseReader(string filePath, Dictionary<string, string> modelMapping, string delimiter = DefaultDelimiter, string culture = DefaultCulture)
        {
            FilePath = filePath;
            Delimiter = delimiter;
            ModelMapping = modelMapping;
            Culture = culture;
            CheckFileExistence();
        }

        private void CheckFileExistence()
        {
            try
            {
                FileUtils.IsFileNotExistThrowAnException(FilePath);
            }
            catch (Exception e)
            {
                throw new FileReaderError($"Could not read file from: {FilePath}.", e);
            }
        }

        public string FilePath { get; set; }
        public string Delimiter { get; set; }
        public Dictionary<string, string> ModelMapping { get; }
        public string Culture { get; }
    }
}
