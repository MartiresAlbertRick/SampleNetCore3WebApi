using System;
using System.Threading.Tasks;
using CommandLine;
using AD.CAAPS.Common.CommandLine;
using NLog;
using AD.CAAPS.Common;

namespace Test.CommandLine
{
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static async Task<int> Main(string[] args)
        {
            Console.WriteLine($"Program CmdLine: {Environment.CommandLine}");
            Console.WriteLine($"Program Args: {string.Join(" ", args)}");
            Console.WriteLine($"Program CurrentDirectory: {Environment.CurrentDirectory}");
            CommandLineUtils.LogHostingFrameworkDetails();
            CommandLineUtils.LogHostingPlatformDetails();
            int exitCode = 1;
            try
            {
                using var parser = CommandLineUtils.GetParser();
                var parseResult = parser.ParseArguments<ProgramOptions>(args);
                exitCode = await parseResult.MapResult(async (ProgramOptions options) =>
                {
                    return (int)(await ProcessCommand(options, args).ConfigureAwait(false));
                },
                    errors => Task.FromResult(2));
            }
            catch (ConfigurationException e)
            {
                logger.Error(e);
                exitCode = (int)3;
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                exitCode = (int)4;
                CommandLineUtils.LogFatalProgramError(e, exitCode);
            }
            finally
            {
                NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
            }
            return exitCode;
        }

        internal static async Task<int> ProcessCommand(ProgramOptions options, string[] args)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            Console.WriteLine("Configuration: " + options.ApplicationConfigurationFileName);
            Console.WriteLine("Connection: " + options.ConnectionStringName);
            Console.WriteLine("Command-line arguments: " + string.Join(' ', args));
            // ProcessCommand(options, args)
            return await Task.FromResult(0).ConfigureAwait(false);
        }
    }
}
