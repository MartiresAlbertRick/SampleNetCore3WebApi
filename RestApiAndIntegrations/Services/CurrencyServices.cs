using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class CurrencyServices : BaseServices, IReadOnlyService<Currency>
    {
        public CurrencyServices(CAAPSDbContext context) : base(context) { }

        public CurrencyServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<Currency> GetMany() => Context.Currencies.AsNoTracking();

        public async Task<Currency> GetOne(int id) => await Context.Currencies.FindAsync(id).ConfigureAwait(false);
    }
}