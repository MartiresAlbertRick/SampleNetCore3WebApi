using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.ErpPaymentRequest.Common;
using AD.CAAPS.Services;
using AD.CAAPS.Common.CommandLine;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AD.CAAPS.ErpPaymentRequest.Urbanise
{
    internal partial class Program
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
                var parseResult = parser.ParseArguments<PaymentRequestProgramOptions>(args);
                exitCode = await parseResult.MapResult(async (PaymentRequestProgramOptions options) => { 
                    return await ExportPaymentRequests(options, args).ConfigureAwait(false); 
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
                NLog.LogManager.Flush();
                NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
            }
            return exitCode;
        }

        private static async Task<int> ExportPaymentRequests(PaymentRequestProgramOptions options, string[] args)
        {
            if (options.Verbose)
            {
                CommandLineUtils.LogHostingFrameworkDetails();
                CommandLineUtils.LogHostingPlatformDetails();
            }
            BuildConfiguration(options, args);
            if (AppSettings == null)
            {
                throw new ConfigurationMissingException("The AppSettings object is not assigned. Check configuration options and ensure <AppSettings> section exists in appsettings.json file");
            }

            BaseServices.ConfigureService(Configuration);

            System.Diagnostics.Trace.WriteLine($"Export maxRecords = {AppSettings.MaxRecords}, dryRun = {AppSettings.DryRun}");
            Console.WriteLine($"Export maxRecords = {AppSettings.MaxRecords}, dryRun = {AppSettings.DryRun}");
            logger.Trace($"Export maxRecords = {AppSettings.MaxRecords}, dryRun = {AppSettings.DryRun}");
            try
            {
                return await new UrbanisePaymentRequest(
                        new DBConfiguration { ConnectionString = ConnectionString, DateFormat = AppSettings.EntityFrameworkDateFormat }, AppSettings)
                        .Export(options.RecordID, AppSettings.MaxRecords, AppSettings.DryRun).ConfigureAwait(false);
                
            }
#pragma warning disable CA1031 // Do not catch general exception types - We are catching and logging all errors and setting ExitCode here.
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                string errorMessage = $"An exception has occured. Message: {e.Message}";
                Console.WriteLine("ERROR: " + errorMessage);
                if (options.Verbose) {
                    // Log entire error message including call-stack:
                    Console.WriteLine("ERROR: " + e.ToString());
                }
                logger.Error(e, errorMessage);
                return (int)ExitCode.UnexpectedError;
            }
        }

        private static void BuildConfiguration(PaymentRequestProgramOptions options, string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                
                builder.SetBasePath(options.WorkingFolder ?? Utils.GetDefaultAppSettingsFilePath());
                
                var switchMappings = new Dictionary<string, string>()
                 {
                     { "-d", "AppSettings:DryRun" },
                     { "-m", "AppSettings:MaxRecords" },
                     { "--dry-run", "AppSettings:DryRun" },
                     { "--max-records", "AppSettings:MaxRecords" },
                     { "--dryRun", "AppSettings:DryRun" },
                     { "--maxRecords", "AppSettings:MaxRecords" }
                 };

                Configuration = builder
                  .AddJsonFile(options.ApplicationConfigurationFileName /* "appsettings.json" */, optional: false, reloadOnChange: false)
                  .AddEnvironmentVariables("AD_CAAPS_PAYMENT_MODULE_")
                  .AddCommandLine(args, switchMappings)
                  .Build();

                // Bind to AppSettings - including type-checking
                AppSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

                ConnectionString = Configuration.GetConnectionString(options.ConnectionStringName ?? "DefaultConnection");

                // Bind NLog configuration
                var NLogConfigSection = Configuration.GetSection("NLog");
                if (NLogConfigSection != null)
                {
                    NLog.LogManager.Configuration = new NLogLoggingConfiguration(NLogConfigSection);
                };
            }
            catch(Exception ex)
            {
                throw new ConfigurationException("Failed to initialize application configuration", ex);
            }
        }
    }
}