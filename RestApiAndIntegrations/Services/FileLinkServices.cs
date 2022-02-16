using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AD.CAAPS.Services
{
    public class FileLinkServices : BaseServices
    {
        public FileLinkServices(CAAPSDbContext context) : base(context) { }

        public FileLinkServices(DBConfiguration configuration) : base(configuration) { }

        public async Task<FileLink> CreateFileLink(FileLink fileLink)
        {
            if (fileLink is null)
            {
                throw new ArgumentNullException(nameof(fileLink));
            }

            int fileIndex = await FileLinkCount(fileLink.RecordId);
            fileLink.FileIndex = fileIndex;
            Context.Add(fileLink);
            await Context.SaveChangesAsync();
            return fileLink;
        }

        public IQueryable<FileLink> GetFileLinks(int recordId)
        {
            return Context.FileLinks.Where(t => t.RecordId == recordId);
        }

        public async Task<int> FileLinkCount(int recordId)
        {
            return await GetFileLinks(recordId).CountAsync();
        }
    }
}