using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class EntityServices : BaseServices, IReadOnlyService<Entity>
    {
        public EntityServices(CAAPSDbContext context) : base(context) { }

        public EntityServices(DBConfiguration dbconfiguration) : base(dbconfiguration) { }

        public async Task<int> GetCount() => await Context.Entities.CountAsync().ConfigureAwait(false);

        public IQueryable<Entity> GetMany() => Context.Entities.AsNoTracking();

        public async Task<Entity> GetOne(int id) => await Context.Entities.FindAsync(id).ConfigureAwait(false);

        public async Task<Entity> CreateEntityAsync(Entity entity)
        {
            if (entity is null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            logger.Debug("Adding entity to the Entities DbSet.", entity);
            Context.Entities.Add(entity);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return await Context.Entities.FindAsync(entity.ID).ConfigureAwait(false);
        }

        public async Task CreateManyEntities(IList<Entity> entities)
        {
            await CreateManyRecords<Entity>(entities).ConfigureAwait(false);
        }

        public async Task DeleteAll()
        {
            logger.Debug($"Removing all Entities from the Entities DbSet");
            await Context.Database.ExecuteSqlRawAsync("DELETE FROM BRE_CAAPS_ENTITY_DETAILS").ConfigureAwait(false);
            // logger.Debug($"Resetting the identity for the Entities DbSet");
            // await Context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('BRE_CAAPS_ENTITY_DETAILS', RESEED, 1)").ConfigureAwait(false);
        }

        public async Task DeleteByEntityCode(string entityCode)
        {
            if (string.IsNullOrWhiteSpace(entityCode))
            {
                throw new System.ArgumentException("The EntityCode parameter must be specified.", nameof(entityCode));
            }
            logger.Debug($"Removing all Entities for entityCode {entityCode} from the Entities DbSet");
            await Context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM BRE_CAAPS_ENTITY_DETAILS WHERE ENTITY_CODE = {0}", entityCode
                            ).ConfigureAwait(false);
        }

        public async Task UpdateEntityAsync(Entity entity)
        {
            Context.Update(entity);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteEntityAsync(Entity entity)
        {
            Context.Entities.Remove(entity);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}