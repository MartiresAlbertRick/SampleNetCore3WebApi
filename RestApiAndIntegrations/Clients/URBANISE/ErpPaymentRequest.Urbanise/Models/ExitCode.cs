using System.ComponentModel;

namespace AD.CAAPS.ErpPaymentRequest.Urbanise
{
    public enum UrbaniseExitCode
    {
        [Description("Unspecified command")]
        UnspecifiedCommandError = -2,

        [Description("Unsupported command")]
        UnsupportedCommandError = -1,

        [Description("Successfully processed all requests")]
        Success = 0,

        [Description("Completed but with failed exports")]
        CompletedButWithFailedExports = 1,

        [Description("No processed records")]
        NoProcessedRecords = 2,

        [Description("Unexpected error")]
        UnexpectedError = 3,

        [Description("Command Line error")]
        CommandLineError = 4,
    }
}