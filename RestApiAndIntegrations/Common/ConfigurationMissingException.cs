using System;
using System.Runtime.Serialization;

namespace AD.CAAPS.Common
{
    [Serializable]
    public class ConfigurationMissingException : Exception
    {
        private readonly string settingName;

        public string SettingName => settingName;

        public ConfigurationMissingException()
        {
        }

        public ConfigurationMissingException(string message) : base(message)
        {
        }

        public ConfigurationMissingException(string settingName, string message) : base(message)
        {
            this.settingName = settingName;
        }

        public ConfigurationMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConfigurationMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}