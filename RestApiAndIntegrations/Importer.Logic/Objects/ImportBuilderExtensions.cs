using AD.CAAPS.Common;
using System.Collections.Generic;

namespace AD.CAAPS.Importer.Logic
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
    public static class ImportBuilderExtensions
    {
        public static IImportBuilder SetImportTypes(this IImportBuilder builder, Dictionary<string, ImportType> collection)
        {
            Utils.CheckDictionaryLengthIsZeroThrowException(collection, () => new ConfigurationException("No value set for \"ImportTypes\" configuration."));
            builder.ImportTypes.Clear();
            foreach (KeyValuePair<string, ImportType> keyValuePair in collection)
            {
                // required
                Utils.CheckStringIsNullOrWhiteSpaceThrowException(keyValuePair.Value.Route, () => new ConfigurationException($"Invalid configuration value for Import Type \"{keyValuePair.Key}\" - \"{nameof(keyValuePair.Value.Route)}\" is empty"));
                Utils.CheckStringIsNullOrWhiteSpaceThrowException(keyValuePair.Value.DefaultImportFilePath, () => new ConfigurationException($"Invalid configuration value for Import Type \"{keyValuePair.Key}\" - \"{nameof(keyValuePair.Value.DefaultImportFilePath)}\" is empty"));
                Utils.CheckStringIsNullOrWhiteSpaceThrowException(keyValuePair.Value.DefaultImportFileName, () => new ConfigurationException($"Invalid configuration value for Import Type \"{keyValuePair.Key}\" - \"{nameof(keyValuePair.Value.DefaultImportFileName)}\" is empty"));
                if (keyValuePair.Value.FileNameIsRegex) Utils.CheckStringIsNullOrWhiteSpaceThrowException(keyValuePair.Value.FileNameDateTimePattern, () => new ConfigurationException($"Invalid configuration value for Import Type \"{keyValuePair.Key}\" - \"{nameof(keyValuePair.Value.FileNameDateTimePattern)}\" is empty"));
                if (keyValuePair.Value.ActionAfterImport == ActionAfterImport.Move) Utils.CheckStringIsNullOrWhiteSpaceThrowException(keyValuePair.Value.TargetFolderAfterImport, () => new ConfigurationException($"Invalid configuration value for Import Type \"{keyValuePair.Key}\" - \"{nameof(keyValuePair.Value.TargetFolderAfterImport)}\" is empty"));
                Utils.CheckDictionaryLengthIsZeroThrowException(keyValuePair.Value.CaapsApiModelDbFieldsMapping, () => new ConfigurationException($"Invalid configuration value for Import Type \"{keyValuePair.Key}\". No value set for \"{nameof(keyValuePair.Value.CaapsApiModelDbFieldsMapping)}\""));
                builder.ImportTypes.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return builder;
        }

        public static IImportBuilder SetApiUrl(this IImportBuilder builder, System.Uri value)
        {
            Utils.CheckObjectIsNullThrowException(value, () => new ConfigurationException("No value set for \"ApiUrl\" configuration."));
            builder.ApiUrl = value;
            return builder;
        }

        public static IImportBuilder SetPostPageSizeLimit(this IImportBuilder builder, int value)
        {
            builder.PostPageSizeLimit = value;
            return builder;
        }

        public static IImportBuilder SetHttpRequestHeaders(this IImportBuilder builder, Dictionary<string, string> collection)
        {
            // parameter value not required
            builder.HttpRequestHeaders.Clear();
            foreach (KeyValuePair<string, string> keyValuePair in collection)
            {
                builder.HttpRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return builder;
        }

        public static IImportBuilder SetHttpRequestParameters(this IImportBuilder builder, Dictionary<string, string> collection)
        {
            // parameter value not required
            builder.HttpRequestHeaders.Clear();
            foreach (KeyValuePair<string, string> keyValuePair in collection)
            {
                builder.HttpRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return builder;
        }

        public static IImportBuilder SetClientSettings(this IImportBuilder builder, ClientSettings value)
        {
            Utils.CheckObjectIsNullThrowException(value, () => new ConfigurationException("No value set for \"ClientSettings\" configuration."));
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(value.ClientId, () => new ConfigurationException($"No value set for \"{nameof(value.ClientId)}\""));
            if (value.AddClientIdAsHttpHeader) Utils.CheckStringIsNullOrWhiteSpaceThrowException(value.ClientIdHttpHeaderName, () => new ConfigurationException($"No value set for \"{nameof(value.ClientIdHttpHeaderName)}\""));
            builder.ClientSettings = value;
            return builder;
        }
    }
}