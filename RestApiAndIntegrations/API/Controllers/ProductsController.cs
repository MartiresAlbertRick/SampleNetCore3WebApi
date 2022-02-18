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
    [Route("api/products")]
    [ApiController]
    public class ProductsController : BaseController
    {
        // GET: api/products
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<Product>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetProducts([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new ProductServices(GetDBConfiguration(GetClient(client))));

        // GET: api/products/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProduct([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new ProductServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/products
        [HttpPost]
        [ProducesResponseType(typeof(List<Product>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostProducts([FromBody] IList<Product> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new ProductServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}