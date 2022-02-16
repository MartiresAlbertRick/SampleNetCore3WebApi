using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class PurchaseOrderServices : BaseServices, IReadOnlyService<PurchaseOrder>
    {
        public PurchaseOrderServices(CAAPSDbContext context) : base(context) { }

        public PurchaseOrderServices(DBConfiguration configuration) : base(configuration) { }

        public async Task<int> GetCount()
        {
            return await Context.PurchaseOrders.CountAsync().ConfigureAwait(false);
        }

        public IQueryable<PurchaseOrder> GetMany() => Context.PurchaseOrders.AsNoTracking();

        public async Task<PurchaseOrder> GetOne(int id) => await Context.PurchaseOrders.FindAsync(id).ConfigureAwait(false);

        public async Task CreateManyPurchaseOrders(List<PurchaseOrder> purchaseOrders, bool truncateTable = false)
        {
            using var transaction = Context.Database.BeginTransaction();
            if (truncateTable)
                await Context.Database.ExecuteSqlRawAsync("DELETE FROM BRE_CAAPS_PURCHASE_ORDERS").ConfigureAwait(false);

            await CreateManyRecords(purchaseOrders).ConfigureAwait(false);
            transaction.Commit();
        }

        public async Task<PurchaseOrder> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder is null)
            {
                throw new System.ArgumentNullException(nameof(purchaseOrder));
            }

            logger.Debug("Adding purchase order to the PurchaseOrders DbSet.", purchaseOrder);
            Context.PurchaseOrders.Add(purchaseOrder);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return await Context.PurchaseOrders.FindAsync(purchaseOrder.ID);
        }

        public async Task UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            logger.Debug("Updating existing purchase order to the PurchaseOrders DbSet.", purchaseOrder);
            Context.Update(purchaseOrder);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            logger.Debug("Removing goods receipt from the PurchaseOrders DbSet.", purchaseOrder);
            Context.PurchaseOrders.Remove(purchaseOrder);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}