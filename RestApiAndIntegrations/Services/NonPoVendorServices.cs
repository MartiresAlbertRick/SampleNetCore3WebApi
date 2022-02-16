using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class NonPoVendorServices : BaseServices, IService<NonPoVendor>
    {
        public NonPoVendorServices(CAAPSDbContext context) : base(context) { }

        public NonPoVendorServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<NonPoVendor> GetMany() => Context.NonPoVendors.AsNoTracking();

        public async Task<NonPoVendor> GetOne(int id) => await Context.NonPoVendors.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<NonPoVendor>> Import(IList<NonPoVendor> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<NonPoVendor>(), GetColumnNames<NonPoVendor>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}