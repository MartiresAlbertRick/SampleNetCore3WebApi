using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class GoodsReceiptServices : BaseServices, IReadOnlyService<GoodsReceipt>
    {
        public GoodsReceiptServices(CAAPSDbContext context) : base(context) { }

        public GoodsReceiptServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<GoodsReceipt> GetMany() => Context.GoodsReceipts.AsNoTracking();

        public async Task<GoodsReceipt> GetOne(int id) => await Context.GoodsReceipts.FindAsync(id).ConfigureAwait(false);

        public async Task CreateManyGoodsReceipts(List<GoodsReceipt> goodsReceipts, bool truncateTable = false)
        {
            using var transaction = Context.Database.BeginTransaction();
            if (truncateTable)
                await Context.Database.ExecuteSqlRawAsync("DELETE FROM BRE_CAAPS_GOODS_RECEIVED").ConfigureAwait(false);
            await CreateManyRecords(goodsReceipts).ConfigureAwait(false);
            transaction.Commit();
        }

        public async Task<GoodsReceipt> CreateGoodsReceipt(GoodsReceipt goodsReceipt)
        {
            if (goodsReceipt is null)
            {
                throw new System.ArgumentNullException(nameof(goodsReceipt));
            }

            logger.Debug("Adding goods receipt to the GoodsReceipt DbSet.", goodsReceipt);
            Context.GoodsReceipts.Add(goodsReceipt);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return await Context.GoodsReceipts.FindAsync(goodsReceipt.ID);
        }

        public async Task UpdateGoodsReceipt(GoodsReceipt goodsReceipt)
        {
            logger.Debug("Updating existing goods receipt to the GoodsReceipts DbSet.", goodsReceipt);
            Context.Update(goodsReceipt);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteGoodsReceipt(GoodsReceipt goodsReceipt)
        {
            logger.Debug("Removing goods receipt from the GoodsReceipts DbSet.", goodsReceipt);
            Context.GoodsReceipts.Remove(goodsReceipt);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}