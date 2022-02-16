using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AD.CAAPS.Importer.Logic
{
    public class ImportCsvReader<T> : BaseReader
    {
        public ImportCsvReader(string filePath, string delimiter, Dictionary<string, string> modelMapping, string culture) : base(filePath, modelMapping, delimiter, culture) { }

        public List<T> ReadFileToModel()
        {
            CsvConfiguration csvReaderConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            using var reader = new StreamReader(FilePath);
            using var csv = new CsvReader(reader, csvReaderConfiguration, true);
            csv.Configuration.Delimiter = Delimiter;
            csv.Configuration.HeaderValidated = null;
            csv.Configuration.MissingFieldFound = null;
            csv.Configuration.TrimOptions = TrimOptions.Trim;
            csv.Configuration.CultureInfo = CultureInfo.GetCultureInfo(Culture);
            RegisterModelMap(csv, ModelMapping);
            List<T> data = csv.GetRecords<T>().ToList();
            return data;
        }

        private void RegisterModelMap(CsvReader csvReader, Dictionary<string, string> map)
        {
            csvReader.Configuration.RegisterClassMap(new CsvMap<T>(map));
        }
    }
}