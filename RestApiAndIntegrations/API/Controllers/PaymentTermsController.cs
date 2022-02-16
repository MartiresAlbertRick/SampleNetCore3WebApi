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
    [Route("api/payment-terms")]
    [ApiController]
    public class PaymentTermsController : BaseController
    {
        // GET: api/payment-terms
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<PaymentTerms>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetPaymentTerms([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new PaymentTermsServices(GetDBConfiguration(GetClient(client))));

        // GET: api/payment-terms/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(PaymentTerms), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetGetPaymentTerm([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new PaymentTermsServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/payment-terms
        [HttpPost]
        [ProducesResponseType(typeof(List<PaymentTerms>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostPaymentTerms([FromBody] IList<PaymentTerms> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "") =>
            await Create(MethodBase.GetCurrentMethod().Name,
                         GetClient(client),
                         dataset,
                         new PaymentTermsServices(GetDBConfiguration(GetClient(client))),
                         truncateTable).ConfigureAwait(false);
    }
}