using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.JsonPatch;
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
    [Route("api/goods-receipts")]
    [ApiController]
    public class GoodsReceiptsController : BaseController
    {
        const string controllerName = "GoodsReceipts";

        // GET: api/goods-receipts
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<GoodsReceipt>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetGoodsReceipts([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new GoodsReceiptServices(GetDBConfiguration(GetClient(client))));

        // GET: api/goods-receipts/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(GoodsReceipt), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetGoodsReceipt([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new GoodsReceiptServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/goods-receipts/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkGoodsReceipts([FromBody] IList<GoodsReceipt> goodsReceipts, [FromQuery] bool truncateTable = false, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POSTBULK";
            try
            {
                if (goodsReceipts is null)
                {
                    throw new ArgumentNullException(nameof(goodsReceipts));
                }
                HandleIncomingPayload(goodsReceipts);
                client = GetClient(client);
                var goodsReceiptServices = new GoodsReceiptServices(GetDBConfiguration(client));
                var grToAdd = new List<GoodsReceipt>();
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = goodsReceiptServices.GetTableName<GoodsReceipt>();
                Dictionary<string, string> uniqueIdentifierMapping = goodsReceiptServices.GetColumnNames<GoodsReceipt>(uniqueIdentifiers.ToArray());
                int grToUpdate = 0;
                foreach (GoodsReceipt goodsReceipt in goodsReceipts)
                {
                    int id = await goodsReceiptServices.CheckExistenceReturnId(goodsReceipt, tableName, uniqueIdentifierMapping);
                    if (id > 0)
                    {
                        grToUpdate++;
                        goodsReceipt.ID = id;
                        await goodsReceiptServices.UpdateGoodsReceipt(goodsReceipt);
                    }
                    else
                        grToAdd.Add(goodsReceipt);
                }
                await goodsReceiptServices.CreateManyGoodsReceipts(grToAdd, truncateTable);
                return Ok($"{grToAdd.Count}/{goodsReceipts.Count} records successfully created. {grToUpdate}/{goodsReceipts.Count} records successfully updated.");
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/goods-receipts
        [HttpPost]
        [ProducesResponseType(typeof(GoodsReceipt), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostGoodsReceipt([FromBody] GoodsReceipt goodsReceipt, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POST";
            try
            {
                if (goodsReceipt is null)
                {
                    throw new ArgumentNullException(nameof(goodsReceipt));
                }
                client = GetClient(client);
                var goodsReceiptServices = new GoodsReceiptServices(GetDBConfiguration(client));
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = goodsReceiptServices.GetTableName<GoodsReceipt>();
                Dictionary<string, string> uniqueIdentifierMapping = goodsReceiptServices.GetColumnNames<GoodsReceipt>(uniqueIdentifiers.ToArray());
                logger.Debug($"{methodName}: Attempting to create or update data", goodsReceipt);
                int id = await goodsReceiptServices.CheckExistenceReturnId(goodsReceipt, tableName, uniqueIdentifierMapping);
                if (id > 0)
                {
                    goodsReceipt.ID = id;
                    await goodsReceiptServices.UpdateGoodsReceipt(goodsReceipt);
                    return StatusCode(201, goodsReceipt);
                }
                else
                {
                    logger.Debug($"{methodName}: Goods receipt successfully created.", goodsReceipt);
                    return StatusCode(201, await goodsReceiptServices.CreateGoodsReceipt(goodsReceipt));
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PUT: api/goods-receipts/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(GoodsReceipt), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutGoodsReceipt([FromRoute] int id, [FromBody] GoodsReceipt goodsReceipt, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-PUT";
            try
            {
                if (goodsReceipt is null)
                {
                    throw new ArgumentNullException(nameof(goodsReceipt));
                }
                if (id != goodsReceipt.ID)
                {
                    string message = $"{methodName}: {nameof(id)} value does not matched with {nameof(goodsReceipt.ID)} value.";
                    logger.Error(message);
                    return BadRequest(message);
                }
                client = GetClient(client);

                var goodsReceiptServices = new GoodsReceiptServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to update goods receipt record.", goodsReceipt);
                await goodsReceiptServices.UpdateGoodsReceipt(goodsReceipt);
                return Ok(goodsReceipt);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PATCH: api/goods-receipts/5
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(GoodsReceipt), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchGoodsReceipt([FromRoute] int id, [FromBody] JsonPatchDocument<GoodsReceipt> patch, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-PATCH";
            try
            {
                if (patch is null)
                {
                    throw new ArgumentNullException(nameof(patch));
                }
                client = GetClient(client);

                var goodsReceiptServices = new GoodsReceiptServices(GetDBConfiguration(client));
                GoodsReceipt goodsReceipt = await goodsReceiptServices.GetOne(id).ConfigureAwait(false);
                if (goodsReceipt == null)
                {
                    string message = $"{methodName}: No goods receipt record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                patch.ApplyTo(goodsReceipt);
                await goodsReceiptServices.UpdateGoodsReceipt(goodsReceipt);
                return Ok(goodsReceipt);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // DELETE: api/goods-receipts/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteGoodsReceipt([FromRoute] int id, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-DELETE";
            try
            {
                client = GetClient(client);

                var goodsReceiptServices = new GoodsReceiptServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to fetch data from goodsReceiptServices.GetGoodsReceipt() with {nameof(id)}: {id}");
                GoodsReceipt goodsReceipt = await goodsReceiptServices.GetOne(id).ConfigureAwait(false);
                if (goodsReceipt == null)
                {
                    string message = $"{methodName}: No goods receipt record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                logger.Debug($"Goods receipt record found for {nameof(id)}: {id}. Attempting to delete goods receipt record.", goodsReceipt);
                await goodsReceiptServices.DeleteGoodsReceipt(goodsReceipt);
                return NoContent();
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
    }
}