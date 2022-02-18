using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace AD.CAAPS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/functions")]
    [ApiController]
    public class FunctionsController : BaseController
    {
        const string controllerName = "FUNCTIONS";

        // GET: api/functions/model-db-mapping/{model}
        [HttpGet("model-db-mapping/{model}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetCaapsApiModelToDbFieldsMapping([FromRoute] string model)
        {
            string methodName = controllerName + "-GET-CAAPSAPI-MODELTODBFIELDSMAPPING";
            try
            {
                var mapping = new Dictionary<string, string>();
                Configuration.GetSection("AppSettings").GetSection(model).GetSection("CaapsApiModelDbFieldsMapping").Bind(mapping);
                return Ok(mapping);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
    }
}