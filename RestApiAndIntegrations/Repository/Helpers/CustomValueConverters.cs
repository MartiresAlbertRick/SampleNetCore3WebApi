using AD.CAAPS.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace AD.CAAPS.Repository
{
    public static class CustomValueConverters
    {
        public static ValueConverter NullableDateTimeConverter(string dateFormat)
        {
            return new ValueConverter<DateTime?, string>
                       (
                           value => Utils.ConvertDateTimeToString(value, dateFormat, true),
                           value => Utils.ConvertStringToDateTime(value, dateFormat)
                       );
        }

        public static ValueConverter NullableStringConverter()
        {
            return new ValueConverter<string, string>
                       (
                            //setting blank/null value default to null
                            value => (string.IsNullOrWhiteSpace(value)) ? null : value,
                            value => (string.IsNullOrWhiteSpace(value)) ? null : value
                       );
        }

        public static ValueConverter NullableDecimalConverter()
        {
            return new ValueConverter<decimal?, string>
                       (
                            value => value.ToString(),
                            value => decimal.Parse(value)
                       );
        }
    }
}