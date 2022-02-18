using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using AD.CAAPS.Entities;
using CsvHelper;
using System.Threading.Tasks;

namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
    class CsvExporter
    {
        private readonly IList<ExportRow> exportRows;
        private readonly CsvWriter writer;
        public CsvExporter(CsvWriter writer, IList<ExportRow> exportRows)
        {
            this.exportRows = exportRows ?? throw new ArgumentNullException(nameof(exportRows));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public async Task ExportAsync()
        {
            writer.WriteHeader<ExportRow>();
            writer.NextRecord();
            await writer.WriteRecordsAsync<ExportRow>(exportRows);
            await writer.FlushAsync();
        }
    }
}
