using AD.CAAPS.Common;
using AD.CAAPS.Common.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AD.CAAPSTaskWrapper
{
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static IConfiguration Configuration { get; private set; } = null;

        private static void BuildConfiguration(string[] args)
        {
            string configPath;
            var cmdLineSwitchMappings = new Dictionary<string, string>()
            {
                 { "--email-sender", "AppSettings:EmailNotifications:ReportSender" },
                 { "--email-notification-recipients", "AppSettings:EmailNotifications:EmailNotificationRecipients" },
                 { "--email-error-recipients", "AppSettings:EmailNotifications:EmailErrorRecipients" }
            };

            configPath = Utils.GetDefaultAppSettingsFilePath();
            var builder = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables("AD_CAAPS_TASK_")
                .AddCommandLine(args, cmdLineSwitchMappings);
            IConfiguration config = builder
                .Build();

            Configuration = config;
            var NLogConfigSection = Configuration.GetSection("NLog");
            if (NLogConfigSection != null)
            {
                NLog.LogManager.Configuration = new NLog.Extensions.Logging.NLogLoggingConfiguration(NLogConfigSection);
            }

            //logging.UseConfiguration(hostingContext.Configuration.GetSection("Logging"));
            //logging.AddConsole();
            //logging.AddDebug();
        }
        
        private static IServiceProvider _serviceProvider;
        private static void RegisterServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<NLog.ILogger>(logger)
                .AddSingleton<IConfiguration>(Configuration)
                .AddSingleton<IMailService, MailService>()
                .AddSingleton<AppHost>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

        // https://devblogs.microsoft.com/dotnet/configureawait-faq/
        // https://pradeeploganathan.com/dotnet/dependency-injection-in-net-core-console-application/
        // https://medium.com/swlh/build-a-command-line-interface-cli-program-with-net-core-428c4c85221
        private static async Task<int> Main(string[] args)
        {
            try
            {
                CommandLineUtils.LogHostingFrameworkDetails();
                CommandLineUtils.LogHostingPlatformDetails();
                BuildConfiguration(args);
                RegisterServices();
                try
                {
                    using IServiceScope Scope = _serviceProvider.CreateScope();
                    return await Scope.ServiceProvider.GetRequiredService<AppHost>()
                        .Run(args)
                        .ConfigureAwait(false);
                }
                finally
                {
                    DisposeServices();
                    NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Program failed: {e}");
                Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
                Console.WriteLine($"ProcessDir: {Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}");
                Console.WriteLine($"AppDomain.CurrentDomain.BaseDir: {AppDomain.CurrentDomain.BaseDirectory}");
                return -1;
            }
        }
    }
}
