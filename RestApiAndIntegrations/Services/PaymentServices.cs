using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class PaymentServices : BaseServices, IReadOnlyService<Payment>
    {
        public PaymentServices(CAAPSDbContext context) : base(context) { }

        public PaymentServices(DBConfiguration configuration) : base(configuration) { }

        public IQueryable<Payment> GetMany() => Context.Payments.AsNoTracking();

        public async Task<Payment> GetOne(int id) => await Context.Payments.FindAsync(id).ConfigureAwait(false);

        public async Task CreateManyPayments(List<Payment> payments, bool truncateTable = false)
        {
            using var transaction = Context.Database.BeginTransaction();
            if (truncateTable)
                await Context.Database.ExecuteSqlRawAsync("DELETE FROM BRE_CAAPS_PAYMENTS").ConfigureAwait(false);
            await CreateManyRecords(payments).ConfigureAwait(false);
            transaction.Commit();
        }

        public async Task<Payment> CreatePayment(Payment payment)
        {
            if (payment is null)
            {
                throw new System.ArgumentNullException(nameof(payment));
            }

            logger.Debug("Adding payment to the Payments DbSet.", payment);
            Context.Payments.Add(payment);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return await Context.Payments.FindAsync(payment.ID);
        }

        public async Task UpdatePayment(Payment payment)
        {
            logger.Debug("Updating existing payment to the Payments DbSet.", payment);
            Context.Update(payment);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeletePayment(Payment payment)
        {
            logger.Debug("Removing payment from the Payments DbSet.", payment);
            Context.Payments.Remove(payment);
            logger.Debug("Attempting to save changes of the CAAPSDbContext.");
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}