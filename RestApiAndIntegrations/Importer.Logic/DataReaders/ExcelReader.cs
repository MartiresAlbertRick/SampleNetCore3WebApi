using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Importer.Logic
{
    public class ExcelReader<T> : BaseReader
    {
        public ExcelReader(string filePath, Dictionary<string, string> modelMapping) : base(filePath, modelMapping) { }

        public List<T> ReadFileToModel()
        {
            FileInfo fileInfo = new FileInfo(FilePath);
            //excel properties
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
            int rows = worksheet.Dimension.Rows;
            int columns = worksheet.Dimension.Columns;
            var headers = new string[columns];
            var collection = new List<Dictionary<string, object>>();
            
            //get headers
            for (int j=1; j<=columns; j++)
            {
                headers[j-1] = worksheet.Cells[1, j].Value.ToString();
            }

            for (int i=2; i<=rows; i++)
            {
                var items = new Dictionary<string, object>();
                for (int j=1; j<=columns; j++)
                {
                    if (ModelMapping.ContainsKey(headers[j - 1]))
                    {
                        items.Add(ModelMapping[headers[j - 1]], worksheet.Cells[i, j].Value);
                    }
                }
                collection.Add(items);
            }

            //this will convert the collection into an object
            //example List<Dictionary<string, object> to List<Vendor>
            string serializedObject = JsonConvert.SerializeObject(collection);

            package.Dispose();

            var deserializedObject = JsonConvert.DeserializeObject<List<T>>(serializedObject);

            return deserializedObject;
        }
    }
}
