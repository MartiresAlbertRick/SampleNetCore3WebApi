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
    [Route("api/units-of-measure")]
    [ApiController]
    public class UnitsOfMeasureController : BaseController
    {
        // GET: api/units-of-measure
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<UnitOfMeasure>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetUnitsOfMeasure([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new UnitOfMeasureServices(GetDBConfiguration(GetClient(client))));

        // GET: api/units-of-measure/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(UnitOfMeasure), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUnitOfMeasure([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new UnitOfMeasureServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/units-of-measure
        [HttpPost]
        [ProducesResponseType(typeof(UnitOfMeasure), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostUnitsOfMeasure([FromBody] IList<UnitOfMeasure> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new UnitOfMeasureServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}