using System;
using System.Collections.Generic;
using System.Text;
using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using NLog;
using System.Threading.Tasks;

namespace AD.CAAPS.ErpPaymentRequest.Common
{
    /// <summary>
    /// This class serves as a common ancestor for all payment request exporters. It holds the necessary parameters and provides their validation.
    /// </summary>
    /// <typeparam name="TAppSettingsClass">Class that contains application settings loaded from program configuration</typeparam>
    public abstract class BasePaymentRequestExporter<TAppSettingsClass> where TAppSettingsClass : BaseAppSettings
    {
        private readonly TAppSettingsClass appSettings;
        private readonly DBConfiguration dbConfiguration;
        private readonly PaymentRequestServices paymentRequestServices;
        protected TAppSettingsClass AppSettings => appSettings;
        protected DBConfiguration DBConfiguration => dbConfiguration;
        protected PaymentRequestServices PaymentRequestServices => paymentRequestServices;
        /// <summary>
        /// Returns a reference to the class logger. Used to allow instantiation of a logger in a specific descendant.
        /// </summary>
        protected abstract Logger Logger { get; }
        public BasePaymentRequestExporter(TAppSettingsClass appSettings, DBConfiguration dbConfiguration, PaymentRequestServicesOptions paymentRequestServicesOptions)
        {
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings), "AppSettings must not be null");
            this.dbConfiguration = dbConfiguration ?? throw new ArgumentNullException(nameof(dbConfiguration), "DB configuration must not be null");
            if (String.IsNullOrWhiteSpace(dbConfiguration.ConnectionString))
                throw new ArgumentException("Connection string must have a value", $"{nameof(dbConfiguration)}.{nameof(dbConfiguration.ConnectionString)}");
            paymentRequestServices = new PaymentRequestServices(dbConfiguration, paymentRequestServicesOptions ?? throw new ArgumentNullException(nameof(paymentRequestServicesOptions), "Payment request services options must not be null"));
        }

        public abstract Task<ExitCode> Export();
    }
}
