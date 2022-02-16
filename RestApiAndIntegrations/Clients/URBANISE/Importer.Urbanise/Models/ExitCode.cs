using System.ComponentModel;

namespace AD.CAAPS.Importer.Urbanise
{
    public enum UrbaniseExitCode
    {
        [Description("Successfully processed all group with no error")]
        Successful = 0,

        [Description("Error on process start")]
        ErrorOnProcessStart = 2,

        [Description("Completed but with error")]
        CompletedButWithErrors = 3,

        [Description("Unexpected error")]
        UnexpectedError = 4,
    }
}