using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper.TypeConversion;
using CsvHelper;
using CsvHelper.Configuration;

namespace AD.CAAPS.ErpPaymentRequest.Common
{

    /// TODO: Move this class into a shared assembly instead of payment request specific one.
    /// <summary>
    /// Converts a DateTime value into a string suitable for exporting into a CSV file as a date.
    /// If the date value is null exports it as an empty string, otherwise CSV exporter exports 
    /// a NULL string instead of nothing.
    /// 
    /// Also handles conversion of dates when they are read from CSV to a DateTime value according to the
    /// specified format, instead of system default.
    /// </summary>
    public class CsvDateConverter : ITypeConverter
    {
        private readonly string dateFormat;

        public CsvDateConverter(string dateFormat)
        {
            this.dateFormat = dateFormat;
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (String.IsNullOrWhiteSpace(text))
                return null;
            else
            {
                if (row is null)
                {
                    throw new ArgumentNullException(nameof(row));
                }
                if (memberMapData is null)
                {
                    throw new ArgumentNullException(nameof(memberMapData));
                }
                if (DateTime.TryParseExact(text, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    return result;
                else
                    throw new InvalidCastException($"Unable to parse the \"{text}\" value into a \"{dateFormat}\" formatted date value. Column: {memberMapData.Index}. Row: {row}.");
            }
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
                return "";
            else
            {
                return String.Format($"{{0:{dateFormat}}}", (DateTime)value);
            }
        }
    }
}
