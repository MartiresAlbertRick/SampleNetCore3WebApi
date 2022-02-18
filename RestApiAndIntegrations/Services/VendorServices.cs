using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class VendorServices : BaseServices, IReadOnlyService<Vendor>
    {
        public VendorServices(CAAPSDbContext context) : base(context) { }

        public VendorServices(DBConfiguration dbconfiguration) : base(dbconfiguration) { }

        public async Task<int> GetCount()
        {
            logger.Trace("Attempting to retrieve record count from CAAPSDbContext - Vendor DbSet.");
            return await Context.Vendors.CountAsync().ConfigureAwait(false);
        }

        public IQueryable<Vendor> GetMany() => Context.Vendors.AsNoTracking();

        public async Task<Vendor> GetOne(int id) => await Context.Vendors.FindAsync(id).ConfigureAwait(false);

        public async Task<Vendor> GetVendorByVendorCode(string vendorCode) 
        {
            return await Context.Vendors.Where(x => x.VendorCode == vendorCode).SingleAsync().ConfigureAwait(false);
        }

        public async Task CreateManyVendors(IList<Vendor> vendors)
        {
            await CreateManyRecords<Vendor>(vendors).ConfigureAwait(false);
        }

        public async Task<Vendor> CreateVendor(Vendor vendor)
        {
            if (vendor is null)
            {
                throw new System.ArgumentNullException(nameof(vendor));
            }

            logger.Debug("Adding vendor to the Vendors DbSet.", vendor);
            Context.Vendors.Add(vendor);
            logger.Trace("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
            logger.Debug($"Saved Vendor.ID: {vendor.ID}.");
            return await Context.Vendors.FindAsync(vendor.ID).ConfigureAwait(false);
        }

        public async Task UpdateVendor(Vendor vendor)
        {
            if (vendor is null)
            {
                throw new System.ArgumentNullException(nameof(vendor));
            }
            logger.Debug($"Updating existing vendor to the Vendors DbSet. ID: {vendor.ID}", vendor);
            Context.Update(vendor);
            logger.Trace("Attempting to save changes of the CAAPSDbContext(Vendors).");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteVendor(Vendor vendor)
        {
            if (vendor is null)
            {
                throw new System.ArgumentNullException(nameof(vendor));
            }
            logger.Debug($"Removing vendor from the Vendors DbSet. ID: {vendor.ID}", vendor);
            Context.Vendors.Remove(vendor);
            logger.Trace("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAll()
        {
            logger.Debug($"Removing all Vendors from the Vendors DbSet");
            await Context.Database.ExecuteSqlRawAsync("DELETE FROM BRE_CAAPS_VENDOR_DETAILS").ConfigureAwait(false);
            // logger.Debug($"Restting the identity for the Vendores DbSet");
            // await Context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('BRE_CAAPS_VENDOR_DETAILS', RESEED, 1)").ConfigureAwait(false);
        }

        public async Task DeleteByEntityCode(string entityCode)
        {
            if (string.IsNullOrWhiteSpace(entityCode))
            {
                throw new System.ArgumentException("The EntityCode parameter must be specified.", nameof(entityCode));
            }
            logger.Debug($"Removing all Vendors for entityCode {entityCode} from the Vendors DbSet");
            await Context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM BRE_CAAPS_VENDOR_DETAILS WHERE ENTITY_CODE = {0}", entityCode
                            ).ConfigureAwait(false);
        }
    }
}