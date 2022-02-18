using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UrlCombineLib;

namespace AD.CAAPS.Common
{
    public static class Utils
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static string ConvertDateTimeToString(DateTime? dateTime, string dateFormat,  bool currentDateIfNull = false)
        {
            if (dateTime == null)
            {
                logger.Trace($"{nameof(dateTime)} is undefined");
                if (currentDateIfNull)
                {
                    return DateTime.Now.ToString(dateFormat);
                }
                return null;
            }
            if (string.IsNullOrWhiteSpace(dateFormat))
            {
                throw new ArgumentException("DateFormat parameter cannot be blank", nameof(dateFormat));
            }

            if (dateTime.HasValue)
            {
                return dateTime?.ToString(dateFormat);
            }
            return null;
        }

        public static DateTime? ConvertStringToDateTime(string dateTime, string dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateTime))
            {
                logger.Debug($"{nameof(dateTime)} is undefined");
                return null;
            }

            if (DateTimeTryParseExact(dateTime, dateFormat, out DateTime outDateTime))
                return outDateTime;
            else
            {
                logger.Debug($"fail to parse argument {nameof(dateTime)} as datetime");
                return null;
            }
        }

        public static bool DateTimeTryParseExact(string dateTime, string dateFormat, out DateTime result)
        {
            return DateTime.TryParseExact(dateTime, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public static char ParseCharSetDefault(string value, char defaultValue)
        {
            if (char.TryParse(value, out char result))
            {
                return result;
            }
            return defaultValue;
        }

        public static string StringFirstNotNull(string value1, string value2)
        {
            if (string.IsNullOrWhiteSpace(value1))
            {
                return value2;
            }

            return value1;
        }

        public static int ParseObjectToInt(object value)
        {
            int parsed = 0;

            if (value == null)
                return parsed;

            if (int.TryParse(value.ToString(), out parsed))
                return parsed;
            else
                return parsed;

        }

        public static int IntFirstNotNull(int? value1, int value2 = 0)
        {
            if (value1 != null)
            {
                return (int)value1;
            }
            return value2;
        }

        public static int GetEnvironmentExitCode()
        {
            return Environment.ExitCode;
        }

        public static void SetEnvironmentExitCode(int exitCode)
        {
            Environment.ExitCode = exitCode;
        }

#pragma warning disable CA1054 // CombineUrl works on strings!
#pragma warning disable CA1055 // CombineUrl works on strings!
        public static string CombineUrl(string baseUrl, params string[] relativePaths)
#pragma warning restore CA1055
#pragma warning restore CA1054
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("URL must be specified", nameof(baseUrl));
            }
            return UrlCombine.Combine(baseUrl, relativePaths);
        }

        public static System.Uri CombineUrl(System.Uri baseUrl, params string[] relativePaths)
        {
            if (baseUrl == null)
            {
                throw new ArgumentException("URL must be specified", nameof(baseUrl));
            }
            return new Uri(UrlCombine.Combine(baseUrl.ToString(), relativePaths));
        }

        public static Uri EnsureTrailingUrlSeparator(Uri baseUrl)
        {
            if (baseUrl is null)
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            string baseAsString = baseUrl.ToString();
            if (string.IsNullOrWhiteSpace(baseAsString))
            {
                throw new ArgumentException("URL must be specified", nameof(baseUrl));
            }
            if (baseAsString.EndsWith("/", StringComparison.InvariantCultureIgnoreCase)) return baseUrl;
            return new Uri(baseAsString + "/");
        }

        public static string StringToCSVCell(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            if ((str.Contains(",", StringComparison.InvariantCultureIgnoreCase) 
                || str.Contains("\"", StringComparison.InvariantCultureIgnoreCase) 
                || str.Contains("\r", StringComparison.InvariantCultureIgnoreCase) 
                || str.Contains("\n", StringComparison.InvariantCultureIgnoreCase)))
                return string.Concat('"', str.Replace("\"", "\"\"", StringComparison.InvariantCultureIgnoreCase), '"');

            return str;
        }

        public static bool IsSuccessStatusCode(int? statusCode)
        {
            return statusCode >= 200 && statusCode < 300;
        }

        public static string ConvertObjectToString(object something)
        {
            if (something == null)
                return "";
            else
                return Convert.ToString(something);
        }

        public static string CleanString(string value)
        {
            return Regex.Replace(value, @"[^0-9a-zA-Z]", "");
        }

        public static string CleanRootFolder(string value)
        {
            return Regex.Replace(value, @"[^A-Z:]", "");
        }

        public static bool IsDigit(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            else
                return value.All(char.IsDigit);
        }

        public static bool IsNumeric(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (int.TryParse(value, out _))
                    return true;
                if (long.TryParse(value, out _))
                    return true;
                if (decimal.TryParse(value, out _))
                    return true;
                if (double.TryParse(value, out _))
                    return true;
            }

            return false;
        }

        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }


        public static int? SafeGetInt(this SqlDataReader reader, int colIndex)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);
            return null;
        }

        public static double? SafeGetDouble(this SqlDataReader reader, int colIndex)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!reader.IsDBNull(colIndex))
                return reader.GetDouble(colIndex);
            return null;
        }

        public static void UpdateJObjectByKey(JObject jsonToken, string key, object value)
        {
            if (jsonToken is null)
            {
                throw new ArgumentNullException(nameof(jsonToken));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key name not specified", nameof(key));
            }
            if (jsonToken.ContainsKey(key))
                jsonToken[key] = JToken.FromObject(value);
            else
                jsonToken.Add(key, JToken.FromObject(value));
        }

        public static bool IsStringJSONObject(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (string.Compare("{", 0, text, 0, 1, true, CultureInfo.InvariantCulture) == 0)
                    return true;
            };
            return false;
        }

        public static string GetDefaultAppSettingsFilePath(string appSettingsFileName = "appsettings.json")
        {
            string configPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            if (!File.Exists(Path.Combine(configPath, appSettingsFileName)))
            {
                configPath = Directory.GetCurrentDirectory();
            }
            return configPath;
        }

        public static void CheckStringIsNullOrWhiteSpaceThrowException(string value, Func<Exception> errorDelegate)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (errorDelegate == null)
                {
                    throw new ArgumentNullException(nameof(errorDelegate), "CheckStringArgumentIsNullOrWhiteSpaceThrowException called with a null error delegate function.");
                }
                Exception e = errorDelegate();
                throw e;
            }
        }

        public static void CheckObjectIsNullThrowException(object value, Func<Exception> errorDelegate)
        {
            if (value is null)
            {
                if (errorDelegate == null)
                {
                    throw new ArgumentNullException(nameof(errorDelegate), "CheckObjectIsNullThrowException called with a null error delegate function.");
                }
                Exception e = errorDelegate();
                throw e;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public static void CheckListLengthIsZeroThrowException<T>(IList<T> value, Func<Exception> errorDelegate)
        {
            CheckObjectIsNullThrowException(value, () => new ArgumentNullException(nameof(value), "The data parameter must be assigned."));
            if (value.Count == 0) //null value checking happening on CheckObjectIsNullThrowException
            {
                if (errorDelegate == null)
                {
                    throw new ArgumentNullException(nameof(errorDelegate), "CheckListLengthIsZeroThrowException called with a null error delegate function.");
                }
                Exception e = errorDelegate();
                throw e;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Checked")]
        public static void CheckDictionaryLengthIsZeroThrowException<T, T2>(Dictionary<T, T2> dictionary, Func<Exception> errorDelegate)
        {
            CheckObjectIsNullThrowException(dictionary, () => new ArgumentNullException(nameof(dictionary), "The parameter must be assigned with value."));
            if (dictionary.Count == 0)
            {
                if (errorDelegate == null)
                {
                    throw new ArgumentNullException(nameof(errorDelegate), "CheckDictionaryLengthIsZeroThrowException called with a null error delegate function.");
                }
                Exception e = errorDelegate();
                throw e;
            }
        }

        public static void CheckValueFromEnumIsInvalidThrowException<T>(object value, Func<Exception> errorDelegate)
        {
            CheckObjectIsNullThrowException(value, () => new ArgumentNullException(nameof(value), "The parameter must be assigned with value."));
            if (!Enum.IsDefined(typeof(T), value))
            {
                if (errorDelegate == null)
                {
                    throw new ArgumentNullException(nameof(errorDelegate), $"{MethodBase.GetCurrentMethod().Name} called with a null error delegate function.");
                }
                Exception e = errorDelegate();
                throw e;
            }
        }

        public static string DisplayEnumValuesAsString<T>()
        {
            string message = "";
            foreach (int key in Enum.GetValues(typeof(T)))
            {
                message += $"{key} - {Enum.GetName(typeof(T), key)}\r\n";
            }
            return message;
        }
    }
}