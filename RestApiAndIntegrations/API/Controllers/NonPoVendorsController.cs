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
    [Route("api/non-po-vendors")]
    [ApiController]
    public class NonPoVendorsController : BaseController
    {
        // GET: api/non-po-vendors
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<NonPoVendor>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetNonPoVendors([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new NonPoVendorServices(GetDBConfiguration(GetClient(client))));

        // GET: api/non-po-vendors/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(NonPoVendor), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetNonPoVendor([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new NonPoVendorServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/non-po-vendors
        [HttpPost]
        [ProducesResponseType(typeof(NonPoVendor), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostNonPoVendors([FromBody] IList<NonPoVendor> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new NonPoVendorServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}