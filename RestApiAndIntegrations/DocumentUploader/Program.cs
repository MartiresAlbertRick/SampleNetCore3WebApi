using AD.CAAPS.Common;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using CommandLine;
using AD.CAAPS.Common.CommandLine;
using System.Threading.Tasks;

namespace AD.CAAPS.DocumentUploader
{
    internal class Program
    {
        private static IConfiguration Configuration;
        private static AppSettings AppSettings;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static async Task<int> Main(string[] args)
        {
            return (int)(await Execute(args));
        }

        public static async Task<ExitCode> Execute(string[] args)
        {
            ExitCode exitCode;
            try
            {
                CommandLineUtils.LogHostingFrameworkDetails();
                CommandLineUtils.LogHostingPlatformDetails();
                int records;
                string docHeader;
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    BuildConfiguration(args);
                    records = Configuration.GetValue<int>("records");
                    docHeader = Configuration.GetValue<string>("docHeader");
                    Console.WriteLine($"Records: {records}");
                    Console.WriteLine($"DocHeader: {docHeader}");

                    var uploader = new Uploader(records, docHeader, AppSettings);
                    exitCode = await uploader.Start().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    logger.Error(e, $"An exception has occured. Message: {e.Message}");
                    exitCode = ExitCode.UnexpectedError;
                }
                stopwatch.Stop();
                logger.Debug($"Elapsed time: {stopwatch.ElapsedMilliseconds}");
            }
            finally
            {
                NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
            }
            return exitCode;
        }

        private static void BuildConfiguration(string[] args)
        {
            Console.WriteLine($"Process: {Process.GetCurrentProcess().ProcessName}");
            Console.WriteLine($"CmdLine: {Process.GetCurrentProcess().MainModule.FileName} {string.Join(" ", args)}");

            Configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             // .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables("AD_CAAPS_DOCUMENT_UPLOADER_")
             .AddCommandLine(args)
             .Build();
            AppSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            var NLogConfigSection = Configuration.GetSection("NLog");
            if (NLogConfigSection != null)
            {
                NLog.LogManager.Configuration = new NLog.Extensions.Logging.NLogLoggingConfiguration(NLogConfigSection);
            }
        }
    }
}