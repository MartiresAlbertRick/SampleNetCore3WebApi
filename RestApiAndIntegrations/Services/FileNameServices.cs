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
    public class FileNameServices : BaseServices
    {
        public FileNameServices(CAAPSDbContext context) : base(context) { }

        public FileNameServices(DBConfiguration configuration) : base(configuration) { }

        public async Task<FileNameRecord> CreateFileName(FileNameRecord fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using (var conn = Context.Database.GetDbConnection())
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    await conn.OpenAsync();

                using var transaction = Context.Database.BeginTransaction();
                fileName.ID = await GetNewId(CAAPSConstants.USP_SEQ_FILENAMES, conn);
                await Context.AddAsync(fileName);
                await Context.SaveChangesAsync();

                transaction.Commit();
            }
            return fileName;
        }

        public async Task<FileNameRecord> GetFileNameAsync(int id)
        {
            return await Context.FileNames.FindAsync(id);
        }
    }
}