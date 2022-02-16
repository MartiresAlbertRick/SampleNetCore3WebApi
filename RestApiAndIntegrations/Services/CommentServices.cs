using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class CommentServices : BaseServices
    {
        public CommentServices(CAAPSDbContext context) : base(context) { }

        public CommentServices(DBConfiguration dbconfiguration) : base(dbconfiguration) { }

        public async Task<Comment> CreateComment(Comment comment)
        {
            if (comment is null)
            {
                throw new System.ArgumentNullException(nameof(comment));
            }

            using (var conn = Context.Database.GetDbConnection())
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();

                using var transaction = Context.Database.BeginTransaction();
                comment.ID = await GetNewId(CAAPSConstants.USP_SEQ_CAAPS_USER_ACTIONS, conn);

                logger.Debug($"Attempting to update comment with comment id {comment.ID}");
                await Context.AddAsync(comment);
                await Context.SaveChangesAsync();
                logger.Debug($"Updating comment successful");

                transaction.Commit();
            }

            return comment;
        }

        public async Task CreateManyComments(IList<Comment> comments, DbConnection connection, DbTransaction transaction)
        {
            if (comments is null)
            {
                throw new System.ArgumentNullException(nameof(comments));
            }

            int[] ids = await GetNewMultipleId(CAAPSConstants.USP_SEQ_CAAPS_USER_ACTIONS, comments.Count, connection, transaction);
            int counter = 0;
            foreach (Comment comment in comments)
            {
                comment.ID = ids[counter]; counter++;
            }
            await CreateManyRecords(comments);
        }
    }
}