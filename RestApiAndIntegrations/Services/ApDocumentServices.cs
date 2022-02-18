using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class ApDocumentServices : BaseServices, IReadOnlyService<ApDocument>
    {
        private readonly SystemOptionServices SystemOptionServices;

        public ApDocumentServices(CAAPSDbContext context) : base(context) {
            SystemOptionServices = new SystemOptionServices(context);
        }

        public ApDocumentServices(DBConfiguration dbconfiguration) : base(dbconfiguration) {
            SystemOptionServices = new SystemOptionServices(dbconfiguration);
        }

        public async Task<ApDocument> CreateApDocument(ApDocument apDocument)
        {
            if (apDocument is null)
            {
                throw new ArgumentNullException(nameof(apDocument));
            }

            using (var conn = Context.Database.GetDbConnection())
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    await conn.OpenAsync();

                using var transaction = Context.Database.BeginTransaction();
                apDocument.ID = await GetNewId(CAAPSConstants.USP_SEQ_DRAWINGS, conn);
                await SetApDocumentDefaultValues(apDocument);
                await Context.AddAsync(apDocument);
                await Context.SaveChangesAsync();
                if (apDocument.LineItems != null && apDocument.LineItems.Count > 0)
                {
                    int[] caapsLineItemIds = await GetNewMultipleId(CAAPSConstants.USP_SEQ_CAAPS_LINE_ITEMS, apDocument.LineItems.Count, conn);

                    int counter = 0;
                    foreach (LineItem lineItem in apDocument.LineItems)
                    {
                        lineItem.ID = caapsLineItemIds[counter];
                        lineItem.RecordId = apDocument.ID;
                        lineItem.LineHeaderUID ??= apDocument.DocHeaderUID;
                        counter++;
                        lineItem.LineNumber = counter;
                    }
                    await CreateManyRecords<LineItem>(apDocument.LineItems);
                }
                if (apDocument.AccountCodingLines != null && apDocument.AccountCodingLines.Count > 0)
                {
                    int[] caapsAccountCodingLineIds = await GetNewMultipleId(CAAPSConstants.USP_SEQ_CAAPS_ACCOUNT_CODING_LINES, apDocument.LineItems.Count);
                    int counter = 0;
                    foreach (GLCodeLine glCodedLineItem in apDocument.AccountCodingLines)
                    {
                        glCodedLineItem.ID = caapsAccountCodingLineIds[counter];
                        glCodedLineItem.RecordId = apDocument.ID;
                        counter++;
                        glCodedLineItem.LineNumber = counter;
                    }
                    await CreateManyRecords<GLCodeLine>(apDocument.AccountCodingLines);
                }

                apDocument.ImportBatchID = 0;
                await Context.SaveChangesAsync();

                transaction.Commit();
            }
            return await Context.ApDocuments.FindAsync(apDocument.ID);
        }

        public IQueryable<ApDocument> GetMany()
        {
            return Context.ApDocuments.AsNoTracking();
        }

        public async Task<ApDocument> GetOne(int id)
        {
            return await Context.ApDocuments.FindAsync(id).ConfigureAwait(false);
        }

        public async Task<ApDocument> GetApDocumentByCaapsRecordId(string CaapsRecordId)
        {
            return await Context.ApDocuments.Where(t => t.CaapsRecordId == CaapsRecordId).AsNoTracking().SingleOrDefaultAsync();
        }

        private async Task SetApDocumentDefaultValues(ApDocument apDocument)
        {
            apDocument.CaapsRecordId = string.Concat("D", apDocument.ID.ToString().PadLeft(8, '0'));
            apDocument.RecordCreatedDate = DateTime.UtcNow;
            apDocument.ProcessStatusCurrent = CAAPSConstants.DEFAULT_PROCESS_STATUS_CURRENT;
            apDocument.UniqueIdentifier = Guid.NewGuid().ToString().ToUpperInvariant();

            SystemOption systemOption = await SystemOptionServices.GetSystemOptionByName(CAAPSConstants.CAAPS_DEFAULT_RECORD_SECURITY);
            apDocument.RoleIDs = systemOption.OptionValue;
        }
    }
}