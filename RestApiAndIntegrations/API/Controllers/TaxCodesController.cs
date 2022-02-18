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
    [Route("api/tax-codes")]
    [ApiController]
    public class TaxCodesController : BaseController
    {
        // GET: api/tax-codes
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<TaxCodeDetails>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetTaxCodes([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new TaxCodeServices(GetDBConfiguration(GetClient(client))));

        // GET: api/tax-codes/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(TaxCodeDetails), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTaxCode([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new TaxCodeServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/tax-codes
        [HttpPost]
        [ProducesResponseType(typeof(List<TaxCodeDetails>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostTaxCodes([FromBody] IList<TaxCodeDetails> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new TaxCodeServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}