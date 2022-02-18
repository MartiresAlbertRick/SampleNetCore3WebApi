using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Authentication;

namespace AD.CAAPS.ErpPaymentRequest.Common
{
    public class FTPSettings
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 990;
        /// <summary>
        /// This configuration property maps to System.Security.Authentication.SslProtocols enum
        /// and represents a comma separated list of SslProtocols values.
        /// 
        /// The default value is "None", which means that it is up to the system to choose the best possible
        /// SSL protocol.
        /// </summary>
        public string SslProtocols { get; set; } = "None";
        /// <summary>
        /// Maps to FluentFTP.FtpEncryptionMode enum value. If an application is using any other FTP client
        /// it should convert the property value to the appropriate FTP client setting.
        /// 
        /// The default value "Implicit" is chosen for the compatibility with the existing code base.
        /// </summary>
        public string EncryptionMode { get; set; } = "Implicit";
    }
}
