using AD.CAAPS.Common.CommandLine;
using CommandLine;

namespace AD.CAAPS.ErpPaymentRequest.Urbanise
{
    internal partial class Program
    {
        public class PaymentRequestProgramOptions : ProgramOptions
        {
            [Option('i', "record-id", HelpText = "Limit the export to a single CAAPS record ID.", Required = false)]
            public int RecordID { get; set; }

            [Option('m', "max-records", HelpText = "Limit the number of payment requests generated.", Required = false, Default = 100)]
            public int MaxRecords { get; set; }
            
            [Option('d', "dry-run", HelpText = "Runs the program without submitting the payment requests and without making any changes to the data.", Required = false)]
            public bool DryRun { get; set; }
        }
    }
}