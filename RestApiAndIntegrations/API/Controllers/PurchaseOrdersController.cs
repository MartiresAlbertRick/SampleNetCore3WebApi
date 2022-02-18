using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AD.CAAPS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/purchase-orders")]
    [ApiController]
    public class PurchaseOrdersController : BaseController
    {
        const string controllerName = "PurchaseOrders";

        // GET: api/purchase-orders
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<PurchaseOrder>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetPurchaseOrders([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new PurchaseOrderServices(GetDBConfiguration(GetClient(client))));

        // GET: api/purchase-orders/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(PurchaseOrder), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPurchaseOrder([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new PurchaseOrderServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/purchase-orders/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkPurchaseOrders([FromBody] IList<PurchaseOrder> purchaseOrders, [FromQuery] bool truncateTable = false, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POSTBULK";
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                if (purchaseOrders is null)
                {
                    throw new ArgumentNullException(nameof(purchaseOrders));
                }
                HandleIncomingPayload(purchaseOrders);
                client = GetClient(client);
                var purchaseOrderServices = new PurchaseOrderServices(GetDBConfiguration(client));
                var poToAdd = new List<PurchaseOrder>();
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = purchaseOrderServices.GetTableName<PurchaseOrder>();
                Dictionary<string, string> uniqueIdentifierMapping = purchaseOrderServices.GetColumnNames<PurchaseOrder>(uniqueIdentifiers.ToArray());
                int poToUpdate = 0;
                foreach (PurchaseOrder purchaseOrder in purchaseOrders)
                {
                    int id = await purchaseOrderServices.CheckExistenceReturnId(purchaseOrder, tableName, uniqueIdentifierMapping);
                    if (id > 0)
                    {
                        poToUpdate++;
                        purchaseOrder.ID = id;
                        await purchaseOrderServices.UpdatePurchaseOrder(purchaseOrder);
                    }
                    else
                        poToAdd.Add(purchaseOrder);
                }
                long validationMilliseconds = stopwatch.ElapsedMilliseconds;
                await purchaseOrderServices.CreateManyPurchaseOrders(poToAdd, truncateTable);
                return FormatOkResponse(poToAdd.Count, poToUpdate, purchaseOrders.Count, await purchaseOrderServices.GetCount(), validationMilliseconds, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/purchase-orders
        [HttpPost]
        [ProducesResponseType(typeof(PurchaseOrder), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostPurchaseOrder([FromBody] PurchaseOrder purchaseOrder, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POST";
            try
            {
                if (purchaseOrder is null)
                {
                    throw new ArgumentNullException(nameof(purchaseOrder));
                }
                BaseServices.UpdateModifiedDateTime(purchaseOrder);
                client = GetClient(client);
                var purchaseOrderServices = new PurchaseOrderServices(GetDBConfiguration(client));
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = purchaseOrderServices.GetTableName<PurchaseOrder>();
                Dictionary<string, string> uniqueIdentifierMapping = purchaseOrderServices.GetColumnNames<PurchaseOrder>(uniqueIdentifiers.ToArray());
                logger.Debug($"{methodName}: Attempting to create or update data", purchaseOrder);
                int id = await purchaseOrderServices.CheckExistenceReturnId(purchaseOrder, tableName, uniqueIdentifierMapping);
                if (id > 0)
                {
                    purchaseOrder.ID = id;
                    await purchaseOrderServices.UpdatePurchaseOrder(purchaseOrder);
                    return StatusCode(201, purchaseOrder);
                }
                else
                {
                    logger.Debug($"{methodName}: Purchase order successfully created.", purchaseOrder);
                    return StatusCode(201, await purchaseOrderServices.CreatePurchaseOrder(purchaseOrder));
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PUT: api/purchase-orders/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PurchaseOrder), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutPurchaseOrder([FromRoute] int id, [FromBody] PurchaseOrder purchaseOrder, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-PUT";
            try
            {
                if (purchaseOrder is null)
                {
                    throw new ArgumentNullException(nameof(purchaseOrder));
                }

                if (id != purchaseOrder.ID)
                {
                    string message = $"{methodName}: {nameof(id)} value does not matched with {nameof(purchaseOrder.ID)} value.";
                    logger.Error(message);
                    return BadRequest(message);
                }
                BaseServices.UpdateModifiedDateTime(purchaseOrder);
                client = GetClient(client);

                var purchaseOrderServices = new PurchaseOrderServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to update purchase order record.", purchaseOrder);
                await purchaseOrderServices.UpdatePurchaseOrder(purchaseOrder);
                return Ok(purchaseOrder);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PATCH: api/purchase-orders/5
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(PurchaseOrder), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchPurchaseOrder([FromRoute] int id, [FromBody] JsonPatchDocument<PurchaseOrder> patch, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-PATCH";
            try
            {
                if (patch is null)
                {
                    throw new ArgumentNullException(nameof(patch));
                }

                client = GetClient(client);

                var purchaseOrderServices = new PurchaseOrderServices(GetDBConfiguration(client));
                PurchaseOrder purchaseOrder = await purchaseOrderServices.GetOne(id).ConfigureAwait(false);
                if (purchaseOrder == null)
                {
                    string message = $"{methodName}: No purchase order record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                BaseServices.UpdateModifiedDateTime(purchaseOrder);
                patch.ApplyTo(purchaseOrder);
                await purchaseOrderServices.UpdatePurchaseOrder(purchaseOrder);
                return Ok(purchaseOrder);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // DELETE: api/purchase-orders/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePurchaseOrder([FromRoute] int id, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-DELETE";

            try
            {
                client = GetClient(client);

                var purchaseOrderServices = new PurchaseOrderServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to fetch data from purchaseOrderServices.GetPurchaseOrder() with {nameof(id)}: {id}");
                PurchaseOrder purchaseOrder = await purchaseOrderServices.GetOne(id).ConfigureAwait(false);
                if (purchaseOrder == null)
                {
                    string message = $"{methodName}: No purchase order record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                logger.Debug($"Purchase order record found for {nameof(id)}: {id}. Attempting to delete purchase order record.", purchaseOrder);
                await purchaseOrderServices.DeletePurchaseOrder(purchaseOrder);
                return NoContent();
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
    }
}