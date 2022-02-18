using CommandLine;
using AD.CAAPS.Common.CommandLine;
using AD.CAAPS.Importer.Common;

namespace AD.CAAPS.Importer
{
    [Verb("run", HelpText = "Run the CSV Importer tool.")]
    public class CsvImporterProgramOptions : ProgramOptions
    {
        [Option('t', "type", HelpText = @"Specifies the object type of import to execute
            0 - RunAllTypes
            1 - Vendor
            2 - Goods Receipt
            3 - Purchase Order
            4 - Import Confirmation
            5 - Payment
            6 - Entity
            7 - Valid Additional Charges
            8 - GL Codes
            9 - Closed Purchase Order
            10 - Non-PO Vendor
            11 - Payment Terms
            12 - Product
            13 - Routing Codes
            14 - Tax Codes
            15 - Unit of Measure", Required = true)]
        public int ImportFileType { get; set; }

        [Option('p', "path", HelpText = "Specifies full path of the import file folder. It will override the current configured file and path.", Required = false, Default = "")]
        public string ImportFilePath { get; set; }
    }
}
