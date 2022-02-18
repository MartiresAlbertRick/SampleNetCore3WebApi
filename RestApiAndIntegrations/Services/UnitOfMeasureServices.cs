using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class UnitOfMeasureServices : BaseServices, IService<UnitOfMeasure>
    {
        public UnitOfMeasureServices(CAAPSDbContext context) : base(context) { }

        public UnitOfMeasureServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<UnitOfMeasure> GetMany() => Context.UnitOfMeasures.AsNoTracking();

        public async Task<UnitOfMeasure> GetOne(int id) => await Context.UnitOfMeasures.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<UnitOfMeasure>> Import(IList<UnitOfMeasure> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<UnitOfMeasure>(), GetColumnNames<UnitOfMeasure>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}