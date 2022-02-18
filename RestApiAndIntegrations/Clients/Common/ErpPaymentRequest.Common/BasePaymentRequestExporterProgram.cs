using System;
using CommandLine;
using AD.CAAPS.Services;
using System.Threading.Tasks;
using AD.CAAPS.Common.CommandLine;
using AD.CAAPS.Common;
using NLog;
using Microsoft.Extensions.Configuration;

namespace AD.CAAPS.ErpPaymentRequest.Common
{
    public abstract class BasePaymentRequestExporterProgram<TAppSettings> where TAppSettings : BaseAppSettings
    {
        protected IConfiguration Configuration { get; set; }
        protected TAppSettings AppSettings { get; set; }
        protected string ConnectionString { get; set; }
        protected abstract Logger Logger { get; }
        public async Task<int> Main(string[] args)
        {
            Console.WriteLine($"Program started: {string.Join(" ", args)}");
            int exitCode = (int)ExitCode.CommandLineError;
            try
            {
                using var parser = CommandLineUtils.GetParser();
                var parseResult = parser.ParseArguments<ProgramOptions>(args);
                exitCode = await parseResult.MapResult(async (ProgramOptions options) => {
                    return (int)(await ExportPaymentRequests(options, args).ConfigureAwait(false));
                },
                    errors => Task.FromResult((int)ExitCode.CompletedButWithErrors));
            }
            catch (ConfigurationException e)
            {
                Logger.Error(e);
                exitCode = (int)ExitCode.ConfigurationError;
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                exitCode = (int)ExitCode.UnexpectedError;
                CommandLineUtils.LogFatalProgramError(e, exitCode);
            }
            finally
            {
                NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
            }
            return exitCode;
        }

        protected abstract BasePaymentRequestExporter<TAppSettings> CreatePaymentRequestExporter();

        private async Task<ExitCode> ExportPaymentRequests(ProgramOptions options, string[] args)
        {
            try
            {
                if (options.Verbose)
                {
                    CommandLineUtils.LogHostingFrameworkDetails();
                    CommandLineUtils.LogHostingPlatformDetails();
                }
                try
                {
                    BuildConfiguration(options, args);
                } catch (Exception e) // Having an exception here means that the logger has not been configured and there is no point
                {
                    Console.Error.WriteLine($"Failed to read configuration. The program cannot continue to work. Exception: {e.Message}");
                    Console.Error.WriteLine();
                    Console.Error.WriteLine(e);
                    Console.Error.WriteLine();
                    foreach (object key in e.Data.Keys)
                    {
                        Console.Error.WriteLine(key + ": " + e.Data[key]);
                    }
                    return ExitCode.ConfigurationError;
                }
                if (AppSettings == null)
                {
                    throw new ConfigurationMissingException("The AppSettings object is not assigned. Check configuration options and ensure <AppSettings> section exists in appsettings.json file");
                }
                BaseServices.ConfigureService(Configuration);
                BasePaymentRequestExporter<TAppSettings> paymentRequestExporter = CreatePaymentRequestExporter();
                return await paymentRequestExporter.Export().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"An exception has occured. Message: {e.Message}");
                return ExitCode.UnexpectedError;
            }
        }

        private void BuildConfiguration(ProgramOptions options, string[] args)
        {
            string workingFolderName = options.WorkingFolder ?? Utils.GetDefaultAppSettingsFilePath();
            string appSettingsFileName = options.ApplicationConfigurationFileName ?? "appsettings.json";

            ConfigurationReadException CreateConfigurationException(string message, Exception innerException = null)
            {
                var configurationException = new ConfigurationReadException(message, innerException);
                configurationException.Data.Add("Working folder", workingFolderName);
                configurationException.Data.Add("Application settings file name", appSettingsFileName);
                configurationException.Data.Add("Command line arguments", String.Join(',', args));
                return configurationException;
            }

            try
            {
                Configuration = new ConfigurationBuilder()
                 .SetBasePath(workingFolderName)
                 .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: false)
                 .AddEnvironmentVariables("AD_CAAPS_PAYMENT_MODULE_")
                 .AddCommandLine(args)
                 .Build();
            } catch (Exception e)
            {
                throw CreateConfigurationException("Unable to build configuration.", e);
            }
            try
            {
                AppSettings = Configuration.GetSection("AppSettings").Get<TAppSettings>();
            } catch (Exception e)
            {
                throw CreateConfigurationException("Unable to initialize application settings from the AppSettings section.", e);
            }
            string connectionString = Configuration.GetConnectionString(options.ConnectionStringName);
            ConnectionString = String.IsNullOrWhiteSpace(connectionString) ? throw CreateConfigurationException($"Unable to get connection string for connection {options.ConnectionStringName}") : connectionString;
            var NLogConfigSection = Configuration.GetSection("NLog");
            if (NLogConfigSection != null)
            {
                NLog.LogManager.Configuration = new NLog.Extensions.Logging.NLogLoggingConfiguration(NLogConfigSection);
            } else
            {
                throw CreateConfigurationException($"Unable to initialize logging configuration. The section NLog is not present in the application configuration file \"{appSettingsFileName}\".");
            }
        }

    }
}
