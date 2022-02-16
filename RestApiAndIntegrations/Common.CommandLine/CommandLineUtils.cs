using CommandLine;
using System;

namespace AD.CAAPS.Common.CommandLine
{
    public static class CommandLineUtils
    {
        public static Parser GetParser(bool ignoreUnknowArguments = true) {
            return new Parser(configuration => {
                configuration.IgnoreUnknownArguments = ignoreUnknowArguments;
                configuration.EnableDashDash = true;
                configuration.CaseSensitive = false;
                configuration.HelpWriter = Console.Error;
            });
        }

        public static void LogHostingPlatformDetails()
        {
            Console.WriteLine(@$"* CLI: `{Environment.CommandLine}`
            * Machine: `{Environment.MachineName}`
            * O/S: `{Environment.OSVersion}`
            * 64bit O/S: `{Environment.Is64BitOperatingSystem}`, 64bit process: `{Environment.Is64BitProcess}`
            * Stack: `{Environment.Version}`
            * Local server time: `{DateTime.Now}`
            * UTC server time: `{DateTime.UtcNow}`");
        }

        public static void LogHostingFrameworkDetails()
        {
            var coreAssemblyInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(object).Assembly.Location);
            Console.WriteLine(@$"Framework product version: {coreAssemblyInfo.ProductVersion}
             File version: {coreAssemblyInfo.FileVersion}
             Location: {typeof(object).Assembly.Location}");
        }


        public static void LogFatalProgramError(Exception e, int exitCode)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            System.Diagnostics.Trace.WriteLine($"ERROR: Program ended with exit code: {exitCode} - {e.Message}");
            Console.Error.WriteLine($"ERROR(S): {exitCode}- {e.Message}");
            Console.Error.WriteLine(e);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
