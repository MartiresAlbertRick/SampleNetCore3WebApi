using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace AD.CAAPS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/import-confirmations")]
    [ApiController]
    public class ImportConfirmationsController : BaseController
    {
        const string controllerName = "ImportConfirmations";

        // GET: api/import-confirmations
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<ImportConfirmation>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetImportConfirmations([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new ImportConfirmationServices(GetDBConfiguration(GetClient(client))));

        // GET: api/import-confirmations/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(ImportConfirmation), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetImportConfirmation([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new ImportConfirmationServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/import-confirmations/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkImportConfirmations([FromBody] IList<ImportConfirmation> importConfirmations, [FromQuery] bool truncateTable = false, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POSTBULK";
            try
            {
                if (importConfirmations is null)
                {
                    throw new ArgumentNullException(nameof(importConfirmations));
                }
                HandleIncomingPayload(importConfirmations);
                client = GetClient(client);
                var importConfirmationServices = new ImportConfirmationServices(GetDBConfiguration(client));
                var importConfirmationToAdd = new List<ImportConfirmation>();
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = importConfirmationServices.GetTableName<ImportConfirmation>();
                Dictionary<string, string> uniqueIdentifierMapping = importConfirmationServices.GetColumnNames<ImportConfirmation>(uniqueIdentifiers.ToArray());
                int importConfirmationToUpdate = 0;
                foreach (ImportConfirmation importConfirmation in importConfirmations)
                {
                    int id = await importConfirmationServices.CheckExistenceReturnId(importConfirmation, tableName, uniqueIdentifierMapping).ConfigureAwait(false);
                    if (id > 0)
                    {
                        importConfirmationToUpdate++;
                        importConfirmation.ID = id;
                        await importConfirmationServices.UpdateImportConfirmation(importConfirmation).ConfigureAwait(false);
                    }
                    else
                        importConfirmationToAdd.Add(importConfirmation);
                }
                await importConfirmationServices.CreateManyImportConfirmations(importConfirmationToAdd, truncateTable).ConfigureAwait(false);
                return Ok($"{importConfirmationToAdd.Count}/{importConfirmations.Count} records successfully created. {importConfirmationToUpdate}/{importConfirmations.Count} records successfully updated.");
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/import-confirmations
        [HttpPost]
        [ProducesResponseType(typeof(ImportConfirmation), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostImportConfirmation([FromBody] ImportConfirmation importConfirmation, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POST";
            try
            {
                if (importConfirmation is null)
                {
                    throw new ArgumentNullException(nameof(importConfirmation));
                }
                client = GetClient(client);
                var importConfirmationServices = new ImportConfirmationServices(GetDBConfiguration(client));
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = importConfirmationServices.GetTableName<ImportConfirmation>();
                Dictionary<string, string> uniqueIdentifierMapping = importConfirmationServices.GetColumnNames<ImportConfirmation>(uniqueIdentifiers.ToArray());
                logger.Debug($"{methodName}: Attempting to create or update data", importConfirmation);
                int id = await importConfirmationServices.CheckExistenceReturnId(importConfirmation, tableName, uniqueIdentifierMapping).ConfigureAwait(false);
                if (id > 0)
                {
                    importConfirmation.ID = id;
                    await importConfirmationServices.UpdateImportConfirmation(importConfirmation).ConfigureAwait(false);
                    return StatusCode((int)HttpStatusCode.Created, importConfirmation);
                }
                else
                {
                    logger.Debug($"{methodName}: Import confirmation successfully created.", importConfirmation);
                    return StatusCode((int)HttpStatusCode.Created, await importConfirmationServices.CreateImportConfirmation(importConfirmation).ConfigureAwait(false));
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // DELETE: api/import-confirmations/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteImportConfirmation([FromRoute] int id, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-DELETE";
            try
            {
                client = GetClient(client);

                var importConfirmationServices = new ImportConfirmationServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to fetch data from importConfirmationServices.GetImportConfirmation() with {nameof(id)}: {id}");
                ImportConfirmation importConfirmation = await importConfirmationServices.GetOne(id).ConfigureAwait(false);
                if (importConfirmation == null)
                {
                    string message = $"{methodName}: No import confirmation record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                logger.Debug($"Import confirmation record found for {nameof(id)}: {id}. Attempting to delete import confirmation record.", importConfirmation);
                await importConfirmationServices.DeleteImportConfirmation(importConfirmation).ConfigureAwait(false);
                return NoContent();
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
    }
}