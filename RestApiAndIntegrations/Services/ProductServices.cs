using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class ProductServices : BaseServices, IService<Product>
    {
        public ProductServices(CAAPSDbContext context) : base(context) { }

        public ProductServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<Product> GetMany() => Context.Products.AsNoTracking();

        public async Task<Product> GetOne(int id) => await Context.Products.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<Product>> Import(IList<Product> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<Product>(), GetColumnNames<Product>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}