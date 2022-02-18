using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class PaymentTermsServices : BaseServices, IService<PaymentTerms>
    {
        public PaymentTermsServices(CAAPSDbContext context) : base(context) { }

        public PaymentTermsServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<PaymentTerms> GetMany() => Context.PaymentTerms.AsNoTracking();

        public async Task<PaymentTerms> GetOne(int id) => await Context.PaymentTerms.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<PaymentTerms>> Import(IList<PaymentTerms> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<PaymentTerms>(), GetColumnNames<PaymentTerms>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}