using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class SystemOptionServices : BaseServices
    {
        public SystemOptionServices(CAAPSDbContext context) : base(context) { }

        public SystemOptionServices(DBConfiguration configuration) : base(configuration) { }

        public async Task<SystemOption> GetSystemOptionByName(string optionName)
        {
            return await Context.SystemOptions.Where(t => t.OptionName == optionName).SingleOrDefaultAsync();
        }
    }
}