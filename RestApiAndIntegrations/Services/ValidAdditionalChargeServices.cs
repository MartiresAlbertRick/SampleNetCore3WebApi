using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class ValidAdditionalChargeServices : BaseServices, IService<ValidAdditionalCharges>
    {
        public ValidAdditionalChargeServices(CAAPSDbContext context) : base(context) { }

        public ValidAdditionalChargeServices(DBConfiguration dbconfiguration) : base(dbconfiguration) { }

        public IQueryable<ValidAdditionalCharges> GetMany() => Context.ValidAdditionalCharges.AsNoTracking();

        public async Task<ValidAdditionalCharges> GetOne(int id) => await Context.ValidAdditionalCharges.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<ValidAdditionalCharges>> Import(IList<ValidAdditionalCharges> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<ValidAdditionalCharges>(), GetColumnNames<ValidAdditionalCharges>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}