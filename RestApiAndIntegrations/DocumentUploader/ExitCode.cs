using System;
using System.ComponentModel;

namespace AD.CAAPS.DocumentUploader
{
  public enum ExitCode
  {
    [Description("Successfully processed all group with no error")]
    Successful = 0,
    [Description("Completed but with some groups unprocessed")]
    CompletedButWithUnprocessed = 1,
    [Description("Completed but with some groups have error")]
    CompletedButWithErrors = 2,
    [Description("Unexpected error")]
    UnexpectedError = 3,
    [Description("Failed to parse command line parameters.")]
    CommandLineError = 4,
    [Description("Failed to load the configuration.")]
    ConfigurationError = 5
  }
}
