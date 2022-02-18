using AD.CAAPS.ErpPaymentRequest.Common;
using AD.CAAPS.Entities;
using NLog;

namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
    class CustomProgram : BasePaymentRequestExporterProgram<AppSettings>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected override Logger Logger => logger;

        protected override BasePaymentRequestExporter<AppSettings> CreatePaymentRequestExporter()
        {
            return new PaymentRequestExporter(AppSettings, new DBConfiguration() { ConnectionString = ConnectionString, DateFormat = AppSettings.EFConfigurationDateFormat });
        }
    }
}
