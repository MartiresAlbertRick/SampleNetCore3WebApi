using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class RoutingCodeServices : BaseServices, IReadOnlyService<RoutingCodes>
    {
        public RoutingCodeServices(CAAPSDbContext context) : base(context) { }

        public RoutingCodeServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<RoutingCodes> GetMany() => Context.RoutingCodes.AsNoTracking();

        public async Task<RoutingCodes> GetOne(int id) => await Context.RoutingCodes.FindAsync(id).ConfigureAwait(false);
    }
}