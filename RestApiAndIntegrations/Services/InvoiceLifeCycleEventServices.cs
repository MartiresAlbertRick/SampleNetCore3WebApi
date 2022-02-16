using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class InvoiceLifeCycleEventServices : BaseServices
    {
        public InvoiceLifeCycleEventServices(CAAPSDbContext context) : base(context) { }

        public InvoiceLifeCycleEventServices(DBConfiguration configuration) : base(configuration) { }

        public async Task UpdateManyLifeCycleEvent(IList<InvoiceLifeCycleEvent> lifeCycleEvents, DbConnection connection, DbTransaction transaction)
        {
            if (lifeCycleEvents is null)
            {
                throw new System.ArgumentNullException(nameof(lifeCycleEvents));
            }

            var existingLifeCycleEventRecords = new List<InvoiceLifeCycleEvent>();
            var nonExistingLifeCycleEventRecords = new List<InvoiceLifeCycleEvent>();

            foreach (InvoiceLifeCycleEvent lifeCycleEvent in lifeCycleEvents)
            {
                InvoiceLifeCycleEvent lifeCycleEventLookup = await Context.InvoiceLifeCycleEvents
                                                                          .Where(t => t.RecordID == lifeCycleEvent.RecordID)
                                                                          .SingleOrDefaultAsync();

                if (lifeCycleEventLookup != null)
                {
                    lifeCycleEventLookup.ExportDate = lifeCycleEvent.ExportDate;
                    lifeCycleEventLookup.ErpImportDate = lifeCycleEvent.ErpImportDate;
                    existingLifeCycleEventRecords.Add(lifeCycleEventLookup);
                }
                else
                {
                    nonExistingLifeCycleEventRecords.Add(lifeCycleEvent);
                }
            }

            await UpdateManyRecordsAsync(existingLifeCycleEventRecords);

            if (nonExistingLifeCycleEventRecords.Count > 1)
            {
                int[] ids = await GetNewMultipleId(CAAPSConstants.USP_SEQ_CAAPS_INVOICE_LIFECYCLE_EVENTS, nonExistingLifeCycleEventRecords.Count, connection, transaction);
                int counter = 0;
                foreach (InvoiceLifeCycleEvent lifeCycleEvent in nonExistingLifeCycleEventRecords)
                {
                    lifeCycleEvent.ID = ids[counter]; counter++;
                }
                await CreateManyRecords(nonExistingLifeCycleEventRecords);
            }
        }
    }
}