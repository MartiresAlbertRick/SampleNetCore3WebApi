using AD.CAAPS.Common;
using AD.CAAPS.Importer.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.ComponentModel;
using System.IO;
using CommandLine;
using System.Threading.Tasks;
using AD.CAAPS.Common.CommandLine;

namespace AD.CAAPS.Importer.Urbanise
{
    internal class Program
    {
        private static IConfiguration Configuration;
        private static AppSettings AppSettings;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static async Task<int> Main(string[] args)
        {
            Console.WriteLine($"Urbanise client data importer started with cmd-line arguments: {string.Join(" ", args)}");
            int exitCode = (int)ExitCode.CommandLineError;
            try
            {
                using var parser = CommandLineUtils.GetParser();
                var parseResult = parser.ParseArguments<ProgramOptions>(args);
                exitCode = await parseResult.MapResult(async (ProgramOptions options) => {
                    return await Import(options, args).ConfigureAwait(false);
                },
                    errors => Task.FromResult((int)ExitCode.CompletedButWithErrors));
                Console.WriteLine($"Program completed with ExitCode: {exitCode}");
                logger.Debug($"Program completed with ExitCode: {exitCode}");
            }
            catch (ConfigurationException e)
            {
                logger.Error(e);
                exitCode = (int)ExitCode.ConfigurationError;
                CommandLineUtils.LogFatalProgramError(e, exitCode);
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

        // [Description("Run api importer. Supported object types are 1 - Vendors and 6 - Entities")]
        private static async Task<int> Import(ProgramOptions options, string[] args)
        {
            if (options.Verbose)
            {
                CommandLineUtils.LogHostingFrameworkDetails();
                CommandLineUtils.LogHostingPlatformDetails();
            }
            BuildConfiguration(options, args);
            try
            {
                ExitCode exitCode = ExitCode.UnexpectedError;
                int objectType = Configuration.GetValue<int>("type");
                Console.WriteLine($"Importing data type: {objectType}");
                logger.Trace($"Importing data type: {objectType}");
                var apiImporter = new APIImporter(AppSettings, (ImportObjectType)objectType);
                Console.Title = $"Importer.Urbanise - Imporing {(ImportObjectType)objectType} details";
                switch ((ImportObjectType)objectType)
                {
                    case ImportObjectType.Vendor:
                        logger.Trace("Import of Vendor details started");
                        exitCode = await apiImporter.Import<Supplier>().ConfigureAwait(false);
                        logger.Trace("Import of Entity details completed");
                        break;
                    case ImportObjectType.Entity:
                        logger.Trace("Import of Vendor details completed");
                        exitCode = await apiImporter.Import<PropertyPlan>().ConfigureAwait(false);
                        logger.Trace("Import of Entity details completed");
                        break;
                    default:
                        throw new NotImplementedException($"No implementation for object type {objectType}. Supported object types are: 1 - Vendor and 6 - Entity");
                }
                return (int)exitCode;
            }
            catch (Exception e)
            {
                logger.Error(e, $"An unexpected error has occured: {e.Message}");
                return (int)ExitCode.UnexpectedError;
            }
        }

        private static void BuildConfiguration(ProgramOptions options, string[] args)
        {
            string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(options.WorkingFolder ?? Utils.GetDefaultAppSettingsFilePath());
            builder
                 .AddJsonFile(options.ApplicationConfigurationFileName ?? "appsettings.json", optional: false, reloadOnChange: false)
                 .AddJsonFile((options.ApplicationConfigurationFileName ?? "appsettings.json").Replace("appsettings.json", $"appsettings.{EnvironmentName}.json", StringComparison.InvariantCultureIgnoreCase), optional: true, reloadOnChange: false)
                 .AddEnvironmentVariables("AD_CAAPS_IMPORTER_")
                 .AddCommandLine(args);

            Configuration = builder.Build();

            AppSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            if (AppSettings == null) 
                throw new ConfigurationMissingException(nameof(AppSettings), "Configuration missing. Check if appsettings.json file exists and contains an AppSettings section.");

            var NLogConfigSection = Configuration.GetSection("NLog");
            if (NLogConfigSection != null)
            {
                NLog.LogManager.Configuration = new NLog.Extensions.Logging.NLogLoggingConfiguration(NLogConfigSection);
            }
        }
    }
}