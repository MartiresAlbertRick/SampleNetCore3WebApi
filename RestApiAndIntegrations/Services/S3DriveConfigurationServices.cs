using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


namespace AD.CAAPS.Services
{
    public class S3DriveConfigurationServices : BaseServices
    {
        public S3DriveConfigurationServices(CAAPSDbContext context) : base(context) { }

        public S3DriveConfigurationServices(DBConfiguration configuration) : base(configuration) { }

        public async Task<S3DriveConfiguration> GetS3DriveConfigurationByPhysicalDriveToken(string physicalDriveToken)
        {
            return await Context.S3DriveConfigurations.Where(t => t.PhysicalDriveToken == physicalDriveToken).SingleOrDefaultAsync();
        }
    }
}