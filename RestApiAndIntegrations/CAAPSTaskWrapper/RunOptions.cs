using CommandLine;
using AD.CAAPS.Common.CommandLine;

namespace AD.CAAPSTaskWrapper
{
    [Verb("run", HelpText = "Runs a command")]
    public class RunOptions : ProgramOptions
    {
        [Option('p', "path", HelpText = "The name of the command-line application to run.", Required = true)]
        public string CmdLine { get; set; }

        [Option('a', "args", HelpText = "The command-line arguments to be passed to the external application.", Required = true)]
        public string Params { get; set; }

        [Option('t', "timeout-ms", Required = false, Default = 0, HelpText = "Timeout (ms) to wait for the process to complete. If not specified, and not set in appsettings.json defaults to 5 minutes.")]
        public int TimeoutMS { get; set; }
    }
}
