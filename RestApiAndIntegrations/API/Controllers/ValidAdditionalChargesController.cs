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
    [Route("api/valid-additional-charges")]
    [ApiController]
    public class ValidAdditionalChargesController : BaseController
    {
        // GET: api/valid-additional-charges
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<ValidAdditionalCharges>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetValidAdditionalCharges([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new ValidAdditionalChargeServices(GetDBConfiguration(GetClient(client))));

        // GET: api/valid-additional-charges/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(ValidAdditionalCharges), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetValidAdditionalCharge([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new ValidAdditionalChargeServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/valid-additional-charges
        [HttpPost]
        [ProducesResponseType(typeof(ValidAdditionalCharges), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostValidAdditionalCharges([FromBody] IList<ValidAdditionalCharges> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new ValidAdditionalChargeServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}