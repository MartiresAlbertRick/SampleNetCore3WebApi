using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using AD.CAAPS.ErpPaymentRequest.Common;
using AD.CAAPS.Common.CommandLine;
using CommandLine;

namespace AD.CAAPS.ErpPaymentRequest.Hallmarc
{
    internal class Program
    {
        private static IConfiguration Configuration;
        private static string ConnectionString;
        private static AppSettings AppSettings;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static async Task<int> Main(string[] args)
        {
            Console.WriteLine($"Program started: {string.Join(" ", args)}");
            int exitCode = (int)ExitCode.CommandLineError;
            try
            {
                using var parser = CommandLineUtils.GetParser();
                var parseResult = parser.ParseArguments<ProgramOptions>(args);
                exitCode = await parseResult.MapResult(async (ProgramOptions options) => {
                    return (int)(await Export(options, args).ConfigureAwait(false));
                },
                    errors => Task.FromResult((int)ExitCode.CompletedButWithErrors));
            }
            catch (ConfigurationException e)
            {
                logger.Error(e);
                exitCode = (int)ExitCode.ConfigurationError;
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                exitCode = (int)ExitCode.UnexpectedError;
                CommandLineUtils.LogFatalProgramError(e, exitCode);
            }
            finally
            {
                NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
            }
            return exitCode;
        }

        private static async Task<ExitCode> Export(ProgramOptions options, string[] args)
        {
            try
            {
                if (options.Verbose)
                {
                    CommandLineUtils.LogHostingFrameworkDetails();
                    CommandLineUtils.LogHostingPlatformDetails();
                }
                BuildConfiguration(options, args);

                var paymentRequest = new PaymentRequest(new DBConfiguration
                                      {
                                          ConnectionString = ConnectionString,
                                          DateFormat = AppSettings.DateFormat
                                      },
                                      AppSettings);
                return await paymentRequest.Export().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.Error(e, $"An exception has occured. Message: {e.Message}");
                return ExitCode.UnexpectedError;
            }
        }

        private static void BuildConfiguration(ProgramOptions options, string[] args)
        {
            Configuration = new ConfigurationBuilder()
             .SetBasePath(options.WorkingFolder ?? Utils.GetDefaultAppSettingsFilePath())
             .AddJsonFile(options.ApplicationConfigurationFileName ?? "appsettings.json", optional: false, reloadOnChange: false)
             .AddEnvironmentVariables("AD_CAAPS_PAYMENT_MODULE_")
             .AddCommandLine(args)
             .Build();

            AppSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            var NLogConfigSection = Configuration.GetSection("NLog");
            if (NLogConfigSection != null)
            {
                NLog.LogManager.Configuration = new NLog.Extensions.Logging.NLogLoggingConfiguration(NLogConfigSection);
            }
        }
    }
}