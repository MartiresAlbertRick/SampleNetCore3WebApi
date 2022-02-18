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
    [Route("api/closed-purchase-orders")]
    [ApiController]
    public class ClosedPurchaseOrdersController : BaseController
    {
        // GET: api/closed-purchase-orders
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<ClosedPurchaseOrder>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetClosedPurchaseOrders([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new ClosedPurchaseOrderServices(GetDBConfiguration(GetClient(client))));

        // GET: api/closed-purchase-orders/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(ClosedPurchaseOrder), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetClosedPurchaseOrder([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new ClosedPurchaseOrderServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/closed-purchase-orders
        [HttpPost]
        [ProducesResponseType(typeof(List<ClosedPurchaseOrder>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostClosedPurchaseOrders([FromBody] IList<ClosedPurchaseOrder> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new ClosedPurchaseOrderServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}