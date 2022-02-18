using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class TaxCodeServices : BaseServices, IService<TaxCodeDetails>
    {
        public TaxCodeServices(CAAPSDbContext context) : base(context) { }

        public TaxCodeServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<TaxCodeDetails> GetMany() => Context.TaxCodes.AsNoTracking();

        public async Task<TaxCodeDetails> GetOne(int id) => await Context.TaxCodes.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<TaxCodeDetails>> Import(IList<TaxCodeDetails> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<TaxCodeDetails>(), GetColumnNames<TaxCodeDetails>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}