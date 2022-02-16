using System;
using System.Text;
using FluentFTP;

namespace AD.CAAPS.Common
{
    public static class StringExtensions
    {
        private static readonly char[] pathDelimiters = new char[] { '\\', '/' };
        public static string Quote(this string value, char quote)
        {
            var builder = new StringBuilder();
            if (string.IsNullOrEmpty(value))
            {
                builder.Append(quote);
                builder.Append(quote);
            }
            else
            {
                builder.Append(quote);
                foreach (char character in value)
                {
                    if (character == quote)
                    {
                        builder.Append(quote);
                        builder.Append(quote);
                    }
                    else
                    {
                        builder.Append(character);
                    }
                }
                builder.Append(quote);
            }
            return builder.ToString();
        }

        public static string Quote(this string value)
        {
            return Quote(value, '\'');
        }

        public static string DoubleQuote(this string value)
        {
            return Quote(value, '\"');
        }

        public static bool HasData(this string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }
        public static string TrimPathDelimiters(this string value)
        {
            return value?.Trim(pathDelimiters);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "As per documentation the format provide parameter is not used.")]
        public static string AppendPathDelimiter(this string value, char pathDelimiter, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (String.IsNullOrEmpty(value))
            {
                return pathDelimiter.ToString();
            }
            else
            {
                if (value.EndsWith(pathDelimiter.ToString(), comparison))
                {
                    return value;
                }
                else
                {
                    return value + pathDelimiter;
                }
            }

        }


        /// <summary>
        /// Encodes string using Base64 encoding.
        /// 
        /// If the string is null, null is returned.
        /// If the string is empty, an empty string is returned.
        /// If the string is neither null nor empty, Base64 encoded string is returned
        /// </summary>
        /// <param name="value">string to encode</param>
        /// <param name="encoding">Encoding to convert the string to byte[] array. If this parameter is omitted, then Encoding.UT8 will be used.</param>
        /// <returns>string encoded using Base64 encoding</returns>
        public static string ToBase64(this string value, Encoding encoding = null)
        {
            if (value == null)
            {
                return null;
            } else if (value.IsBlank())
            {
                return "";
            } else
            {
                Encoding actualEncoding = encoding ?? Encoding.UTF8;
                byte[] bytes = actualEncoding.GetBytes(value);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
