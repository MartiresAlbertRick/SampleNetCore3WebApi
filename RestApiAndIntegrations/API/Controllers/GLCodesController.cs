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
    [Route("api/gl-codes")]
    [ApiController]
    public class GLCodesController : BaseController
    {
        // GET: api/gl-codes
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<GLCodeDetails>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public  IActionResult GetGLCodes([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new GLCodeServices(GetDBConfiguration(GetClient(client))));

        // GET: api/gl-codes/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(GLCodeDetails), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetGLCode([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new GLCodeServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/gl-codes
        [HttpPost]
        [ProducesResponseType(typeof(List<GLCodeDetails>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostGLCodes([FromBody] IList<GLCodeDetails> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new GLCodeServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}