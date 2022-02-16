using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class ClosedPurchaseOrderServices : BaseServices, IService<ClosedPurchaseOrder>
    {
        public ClosedPurchaseOrderServices(CAAPSDbContext context) : base(context) { }

        public ClosedPurchaseOrderServices(DBConfiguration dbconfiguration) : base(dbconfiguration) { }

        public IQueryable<ClosedPurchaseOrder> GetMany() => Context.ClosedPurchaseOrders.AsNoTracking();

        public async Task<ClosedPurchaseOrder> GetOne(int id) => await Context.ClosedPurchaseOrders.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<ClosedPurchaseOrder>> Import(IList<ClosedPurchaseOrder> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<ClosedPurchaseOrder>(), GetColumnNames<ClosedPurchaseOrder>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}