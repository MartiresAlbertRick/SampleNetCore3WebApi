using AD.CAAPS.Common;
using AD.CAAPS.Common.CommandLine;
using AD.CAAPS.EmailServices;
using AD.CAAPS.Importer.Logic;
using AD.CAAPS.Importer.Common;
using CommandLine;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace AD.CAAPS.Importer
{
    internal static class Program
    {
        private static AppSettings AppSettings { get; set; }

        private static async Task<int> Main(string[] args)
        {
            Console.WriteLine($"Program started: {string.Join(" ", args)}");
            var stopwatch = Stopwatch.StartNew();

            int exitCode = (int)ExitCode.CommandLineError;
            try
            {
                using var parser = CommandLineUtils.GetParser(false);
                var parseResult = parser.ParseArguments<CsvImporterProgramOptions>(args);
                exitCode = await parseResult.MapResult(async (CsvImporterProgramOptions options) =>
                                    {
                                        return await Import(options).ConfigureAwait(false);
                                    },
                                    errors => Task.FromResult((int)ExitCode.CommandLineError)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (e is ConfigurationException) {
                    logger.Error(e);
                    exitCode = (int)ExitCode.ConfigurationError;
                } else if (e is InvalidImportTypeException) {
                    logger.Error(e);
                    exitCode = (int)ExitCode.CommandLineError;
                } else {
                    logger.Fatal(e);
                    exitCode = (int)ExitCode.UnexpectedError;
                }
                CommandLineUtils.LogFatalProgramError(e, exitCode);
            }
            finally
            {
                stopwatch.Stop();
                logger.Info($"The program has exited with code {exitCode}");
                logger.Info($"Time elapsed {stopwatch.ElapsedMilliseconds:N0}ms");
                LogManager.Shutdown(); // Flush and close down internal threads and timers
            }
            return exitCode;
        }

        //nlog implementation - see example https://github.com/NLog/NLog.Extensions.Logging/tree/master/examples/NetCore2/ConsoleExample
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static IConfiguration Configuration { get; private set; } = null;

        private static async Task<int> Import(CsvImporterProgramOptions options)
        {
            if (options.Verbose)
            {
                CommandLineUtils.LogHostingFrameworkDetails();
                CommandLineUtils.LogHostingPlatformDetails();
            }
            BuildConfiguration(options);
            var builder = new ImportBuilder();
            logger.Debug($"Importer is executed with parameters type \"{options.ImportFileType}\" and file path \"{options.ImportFilePath}\"");
            Utils.CheckValueFromEnumIsInvalidThrowException<ImportObjectType>(options.ImportFileType, () => new InvalidImportTypeException($"The selected import type \"{options.ImportFileType}\" is invalid.\r\n{Utils.DisplayEnumValuesAsString<ImportObjectType>()}"));
            ImportObjectType objectType = (ImportObjectType)options.ImportFileType;
            try
            {
                builder.SetImportTypes(AppSettings.ImportTypes)
                        .SetApiUrl(AppSettings.ApiUrl)
                        .SetHttpRequestHeaders(AppSettings.HttpRequestHeaders)
                        .SetPostPageSizeLimit(AppSettings.PostPageSizeLimit)
                        .SetClientSettings(AppSettings.ClientSettings);
                var importer = new ImportService(builder, objectType, options.ImportFilePath);
                ExitCode exitCode = await importer.Start().ConfigureAwait(false);
                return (int)exitCode;
            }
            catch (Exception e)
            {
                HandleProgramException(objectType, builder, e);
                throw;
            }
        }

        private static void BuildConfiguration(CsvImporterProgramOptions options)
        {
            logger.Debug($"Reading configuration file from {Directory.GetCurrentDirectory()}");
            var builder = new ConfigurationBuilder();
            try {
                builder.SetBasePath(options.WorkingFolder ?? Utils.GetDefaultAppSettingsFilePath());
                builder.AddJsonFile(options.ApplicationConfigurationFileName, optional: false, reloadOnChange: false)
                    .AddEnvironmentVariables("AD_CAAPS_IMPORTER_");

                Configuration = builder.Build();
                AppSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
                if (AppSettings == null)
                {
                    throw new ConfigurationException($"Configuration missing for {nameof(AppSettings)}. Check if appsettings.json file exists and contains an AppSettings section.");
                }

                var NLogConfigSection = Configuration.GetSection("NLog");
                if (NLogConfigSection != null)
                {
                    NLog.LogManager.Configuration = new NLog.Extensions.Logging.NLogLoggingConfiguration(NLogConfigSection);
                }
            }
            catch (Exception e) {
                // this is to ensure that it will be thrown as ConfigurationException to be decoded as ExitCode.ConfigurationError
                throw new ConfigurationException($"An exception caught while running {MethodBase.GetCurrentMethod().Name}", e);
            }
            
        }

        private static void HandleProgramException(ImportObjectType objectType, IImportBuilder builder, Exception e)
        {
            logger.Error(e, $"Error occurred: {e.Message}");
            if (e is AggregateException)
            {
                foreach (var ex in (e as AggregateException).InnerExceptions)
                {
                    logger.Error($"Aggregate exceptions: {ex.Message}");
                }
            }
            SendErrorNotification(objectType, builder, e);
        }

        public static void SendErrorNotification(ImportObjectType objectType, IImportBuilder builder, Exception e)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder), "The IImportBuilder parameter must be assigned.");
            if (e is null) throw new ArgumentNullException(nameof(e), "The Exception parameter must be assigned.");
            try
            {
                var options = new SendGridOptions()
                {
                    EmailSender = Configuration["AppSettings:EmailNotifications:EmailSender"],
                    EmailNotificationRecipients = Configuration["AppSettings:EmailNotifications:EmailNotificationRecipients"],
                    EmailErrorRecipients = Configuration["AppSettings:EmailNotifications:EmailErrorRecipients"],
                    SendGridAPIKey = Configuration["AppSettings:SendGridAPIKey"]
                };
                if (string.IsNullOrWhiteSpace(options.EmailSender)) {
                    logger.Error($"Email notifications disabled - AppSettings:EmailNotifications:EmailSender not configured");
                    return;
                }
                if (string.IsNullOrWhiteSpace(options.EmailErrorRecipients))
                {
                    logger.Error($"Email notifications disabled - AppSettings:EmailNotifications:EmailErrorRecipients not configured");
                    return;
                }
                if (string.IsNullOrWhiteSpace(options.SendGridAPIKey))
                {
                    logger.Error($"Email notifications disabled - AppSettings:SendGridAPIKey not configured");
                    return;
                }
                string ClientId = Configuration["AppSettings:ClientSettings:ClientId"] ?? "Unknown";
                
                using var notification = new SendGridSender(options);
                notification.Message.Subject = $"Error importing {objectType} data for {ClientId}";
                notification.AppendLine($"# Error importing {objectType} data");
                notification.AppendLine("");
                notification.AppendLine($"* Client: \"`{ClientId}`\"");
                notification.AppendLine("");
                notification.AppendLine($"* CAAPS API: \"`{builder.ApiUrl}`\"");
                notification.AppendLine("");
                notification.AppendLine($"* Local time: \"`{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}`\"");
                notification.AppendLine("");
                notification.AppendLine("### Error details");
                notification.AppendLine("");
                notification.AppendLine("```");
                notification.AppendLine(e.ToString());
                notification.AppendLine("```");
                notification.AppendLine("");
                notification.AddProcessEnvironmentDetails();
                notification.BuildMessageHtmlBody();
                var response = notification.SendEmail(true).Result;
                if (!response.Sent)
                {
                    logger.Error($"Email notification failed: {response.ErrorDetails}");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                logger.Error(ex, $"Email notification failed: {ex.Message}");
            }
        }
    }
}
