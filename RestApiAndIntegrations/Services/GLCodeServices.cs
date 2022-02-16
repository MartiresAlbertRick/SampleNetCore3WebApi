using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class GLCodeServices : BaseServices, IService<GLCodeDetails>
    {
        public GLCodeServices(CAAPSDbContext context) : base(context) { }

        public GLCodeServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<GLCodeDetails> GetMany() => Context.GLCodes.AsNoTracking();

        public async Task<GLCodeDetails> GetOne(int id) => await Context.GLCodes.FindAsync(id).ConfigureAwait(false);

        public async Task<IList<GLCodeDetails>> Import(IList<GLCodeDetails> dataset,
                                                     string[] uniqueIdentifierMapping,
                                                     bool truncateTable = false,
                                                     bool generateSequence = false,
                                                     string generateSequenceStoredProcedureName = null)
            => await ImportAsync(dataset, GetTableName<GLCodeDetails>(), GetColumnNames<GLCodeDetails>(uniqueIdentifierMapping), truncateTable).ConfigureAwait(false);
    }
}