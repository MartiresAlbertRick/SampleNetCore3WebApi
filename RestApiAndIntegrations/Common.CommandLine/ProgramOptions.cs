using CommandLine;

namespace AD.CAAPS.Common.CommandLine
{
    public class ProgramOptions
    {
        [Option('s', "appsettings", HelpText = "Specifies full name of the application configuration file name. If omitted a default configuration file will be used.", Required = false, Default = "appsettings.json")]
        public string ApplicationConfigurationFileName { get; set; } = "appsettings.json";

        [Option('c', "connection", HelpText = "Specifies a name of the connection string in the configuration file that should be used for connection to a client's database.", Required = false, Default = "DefaultConnection")]
        public string ConnectionStringName { get; set; } = "DefaultConnection";

        [Option('w', "working-folder", HelpText = "Specifies full path of the working folder. The configuration and any other files will be loaded from this base folder.", Required = false, Default = null)]
        public string WorkingFolder { get; set; } = null;

        [Option('v', "verbose", HelpText = "Print verbose error messages including callstacks.", Required = false, Default = false)]
        public bool Verbose { get; set; } = false;
    }
}