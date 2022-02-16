using System.ComponentModel;

namespace AD.CAAPS.Importer.Logic
{
    public enum ExitCode
    {
        [Description("Successfull import completed")]
        SuccessfulNoError = 0,

        [Description("Import consumer error")]
        ImportConsumerError = 1,

        [Description("Import service error")]
        ImportServiceError = 2,

        [Description("CAAPS API non-success status code")]
        CaapsApiError = 3,

        [Description("Failed to parse command line parameters.")]
        CommandLineError = 4,

        [Description("Failed to load the configuration.")]
        ConfigurationError = 5,

        [Description("Error while reading the import file")]
        FileReaderError = 6,

        [Description("Unexpected error")]
        UnexpectedError = 7,
        
        [Description("Completed but with failed imports")]
        CompletedWithFailedImport = 8
    }
}