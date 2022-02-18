using System;
using System.Collections.Generic;
using System.Text;

namespace AD.CAAPS.Entities
{
    public static class CAAPSConstants
    {
        // system options constants
        public const string CAAPS_DEFAULT_RECORD_SECURITY = "CAAPS Default Record Security",
                            IMAGE_DIRECTORY = "Image Directory";

        public const string BRE_CAAPS_VENDOR_DETAILS = "BRE_CAAPS_VENDOR_DETAILS",
                            BRE_CAAPS_PURCHASE_ORDERS = "BRE_CAAPS_PURCHASE_ORDERS",
                            BRE_CAAPS_GOODS_RECEIVED = "BRE_CAAPS_GOODS_RECEIVED";

        // sequence stored procedures
        public const string USP_SEQ_DRAWINGS = "USP_SEQ_DRAWINGS",
                            USP_SEQ_CAAPS_LINE_ITEMS = "USP_SEQ_CAAPS_LINE_ITEMS",
                            USP_SEQ_CAAPS_ACCOUNT_CODING_LINES = "USP_SEQ_CAAPS_ACCOUNT_CODING_LINES",
                            USP_SEQ_CAAPS_USER_ACTIONS = "USP_SEQ_CAAPS_USER_ACTIONS",
                            USP_SEQ_FILENAMES = "USP_SEQ_FILENAMES",
                            USP_SEQ_CAAPS_INVOICE_LIFECYCLE_EVENTS = "USP_SEQ_CAAPS_INVOICE_LIFECYCLE_EVENTS";

        // document default values
        public const string DEFAULT_PROCESS_STATUS_CURRENT = "IMPORTED";
        public const int DEFAULT_FILE_INDEX = 0,
                         DEFAULT_MANAGEDTABLEID = 0;

        // file extensions
        public const string PDF_EXTENSION = ".PDF";

        // document status
        public const string PROCESS_STATUS_CURRENT_EXPORT_FAILED = "EXPORT FAILED";
    }
}