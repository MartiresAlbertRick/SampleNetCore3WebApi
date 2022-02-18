using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class ImportConfirmationServices : BaseServices, IReadOnlyService<ImportConfirmation>
    {
        public ImportConfirmationServices(CAAPSDbContext context) : base(context) { }

        public ImportConfirmationServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<ImportConfirmation> GetMany() => Context.ImportConfirmations.AsNoTracking();

        public async Task<ImportConfirmation> GetOne(int id) => await Context.ImportConfirmations.FindAsync(id).ConfigureAwait(false);

        public async Task CreateManyImportConfirmations(List<ImportConfirmation> importConfirmations, bool truncateTable = false)
        {
            using var transaction = Context.Database.BeginTransaction();
            if (truncateTable)
                await Context.Database.ExecuteSqlRawAsync("DELETE FROM BRE_CUSTOM_IMPORT_CONFIRMATIONS").ConfigureAwait(false);
            await CreateManyRecords(importConfirmations).ConfigureAwait(false);
            transaction.Commit();
        }

        public async Task<ImportConfirmation> CreateImportConfirmation(ImportConfirmation importConfirmation)
        {
            if (importConfirmation is null)
            {
                throw new System.ArgumentNullException(nameof(importConfirmation));
            }

            logger.Debug("Adding import confirmation to the ImportConfirmations DbSet.", importConfirmation);
            Context.ImportConfirmations.Add(importConfirmation);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return await Context.ImportConfirmations.FindAsync(importConfirmation.ID).ConfigureAwait(false);
        }

        public async Task UpdateImportConfirmation(ImportConfirmation importConfirmation)
        {
            logger.Debug("Updating existing import confirmation to the Import Confirmation DbSet.", importConfirmation);
            Context.Update(importConfirmation);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteImportConfirmation(ImportConfirmation importConfirmation)
        {
            logger.Debug("Removing goods receipt from the ImportConfirmations DbSet.", importConfirmation);
            Context.ImportConfirmations.Remove(importConfirmation);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}