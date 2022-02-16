using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AD.CAAPS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/payment-updates")]
    [ApiController]
    public class PaymentUpdatesController : BaseController
    {
        const string controllerName = "Payments";

        // GET: api/payment-updates
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<Payment>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetPayments([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new PaymentServices(GetDBConfiguration(GetClient(client))));

        // GET: api/payment-updates/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(Payment), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPayment([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new PaymentServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/payment-updates/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkPayments([FromBody] IList<Payment> payments, [FromQuery] bool truncateTable = false, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POSTBULK";
            try
            {
                if (payments is null)
                {
                    throw new ArgumentNullException(nameof(payments));
                }
                HandleIncomingPayload(payments);
                client = GetClient(client);
                var paymentServices = new PaymentServices(GetDBConfiguration(client));
                var paymentToAdd = new List<Payment>();
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = paymentServices.GetTableName<Payment>();
                Dictionary<string, string> uniqueIdentifierMapping = paymentServices.GetColumnNames<Payment>(uniqueIdentifiers.ToArray());
                int paymentToUpdate = 0;
                foreach (Payment payment in payments)
                {
                    int id = await paymentServices.CheckExistenceReturnId(payment, tableName, uniqueIdentifierMapping);
                    if (id > 0)
                    {
                        paymentToUpdate++;
                        payment.ID = id;
                        await paymentServices.UpdatePayment(payment);
                    }
                    else
                        paymentToAdd.Add(payment);
                }
                await paymentServices.CreateManyPayments(paymentToAdd, truncateTable);
                return Ok($"{paymentToAdd.Count}/{payments.Count} records successfully created. {paymentToUpdate}/{payments.Count} records successfully updated.");
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/payment-updates
        [HttpPost]
        [ProducesResponseType(typeof(Payment), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostPayment([FromBody] Payment payment, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POST";
            try
            {
                if (payment is null)
                {
                    throw new ArgumentNullException(nameof(payment));
                }
                client = GetClient(client);
                var paymentServices = new PaymentServices(GetDBConfiguration(client));
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = paymentServices.GetTableName<Payment>();
                Dictionary<string, string> uniqueIdentifierMapping = paymentServices.GetColumnNames<Payment>(uniqueIdentifiers.ToArray());
                logger.Debug($"{methodName}: Attempting to create or update data", payment);
                int id = await paymentServices.CheckExistenceReturnId(payment, tableName, uniqueIdentifierMapping);
                if (id > 0)
                {
                    payment.ID = id;
                    await paymentServices.UpdatePayment(payment);
                    return StatusCode(201, payment);
                }
                else
                {
                    logger.Debug($"{methodName}: Payment successfully created.", payment);
                    return StatusCode(201, await paymentServices.CreatePayment(payment));
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // DELETE: api/payment-updates/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePayment([FromRoute] int id, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-DELETE";
            try
            {
                client = GetClient(client);

                var paymentServices = new PaymentServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to fetch data from paymentServices.GetPayment() with {nameof(id)}: {id}");
                Payment payment = await paymentServices.GetOne(id).ConfigureAwait(false);
                if (payment == null)
                {
                    string message = $"{methodName}: No payment record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                logger.Debug($"Payment record found for {nameof(id)}: {id}. Attempting to delete payment record.", payment);
                await paymentServices.DeletePayment(payment);
                return NoContent();
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
    }
}