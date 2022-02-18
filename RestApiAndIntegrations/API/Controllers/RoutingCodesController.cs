using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AD.CAAPS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/routing-codes")]
    [ApiController]
    public class RoutingCodesController : BaseController
    {
        // GET: api/routing-codes
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<RoutingCodes>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetRoutingCodes([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new RoutingCodeServices(GetDBConfiguration(GetClient(client))));

        // GET: api/routing-codes/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(RoutingCodes), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRoutingCode([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new RoutingCodeServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);
    }
}