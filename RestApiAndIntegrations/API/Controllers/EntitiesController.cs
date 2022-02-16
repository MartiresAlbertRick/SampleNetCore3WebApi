using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AD.CAAPS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/entities")]
    [ApiController]
    public class EntitiesController : BaseController
    {
        const string controllerName = "Entities";

        // GET: api/entities
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<Entity>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetEntities([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new EntityServices(GetDBConfiguration(GetClient(client))));

        // GET: api/entities/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(Entity), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetEntity([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new EntityServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/entities
        [HttpPost]
        [ProducesResponseType(typeof(Entity), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostEntity([FromBody] Entity entity, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POST";
            try
            {
                if (entity is null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                client = GetClient(client);
                var entityServices = new EntityServices(GetDBConfiguration(client));
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = entityServices.GetTableName<Entity>();
                Dictionary<string, string> uniqueIdentifierMapping = entityServices.GetColumnNames<Entity>(uniqueIdentifiers.ToArray());
                BaseServices.UpdateModifiedDateTime(entity);
                logger.Debug($"{methodName}: Attempting to create or update data", entity);
                int id = await entityServices.CheckExistenceReturnId(entity, tableName, uniqueIdentifierMapping);
                if (id > 0)
                {
                    entity.ID = id;
                    await entityServices.UpdateEntityAsync(entity);
                    return StatusCode(201, entity);
                }
                else
                {
                    logger.Debug($"{methodName}: Entity successfully created.", entity);
                    return StatusCode(201, await entityServices.CreateEntityAsync(entity));
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/entities/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkEntities([FromBody] IList<Entity> dataset, [FromQuery] bool truncateTable = false, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POSTBULK";
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                if (dataset is null)
                {
                    throw new ArgumentNullException(nameof(dataset));
                }
                HandleIncomingPayload(dataset);
                client = GetClient(client);
                var entityServices = new EntityServices(GetDBConfiguration(client));
                using var transaction = entityServices.BeginTransaction();
                try
                {
                    var entitiesToAdd = new List<Entity>();
                    int entitiesToUpdate = 0;
                    if (truncateTable)
                    {
                        await entityServices.DeleteAll().ConfigureAwait(false);
                        entitiesToAdd.AddRange(dataset);
                    }
                    else
                    {
                        List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                        string tableName = entityServices.GetTableName<Entity>();
                        Dictionary<string, string> uniqueIdentifierMapping = entityServices.GetColumnNames<Entity>(uniqueIdentifiers.ToArray());
                        foreach (Entity entity in dataset)
                        {
                            int id = await entityServices.CheckExistenceReturnId(entity, tableName, uniqueIdentifierMapping, null, transaction);
                            if (id > 0)
                            {
                                entitiesToUpdate++;
                                entity.ID = id;
                                await entityServices.UpdateEntityAsync(entity);
                            }
                            else
                                entitiesToAdd.Add(entity);
                        }
                    }
                    long ValidationMilliseconds = stopwatch.ElapsedMilliseconds;
                    await entityServices.CreateManyEntities(entitiesToAdd);
                    await EntityServices.CommitTransaction(transaction).ConfigureAwait(false);
                    return FormatOkResponse(entitiesToAdd.Count, entitiesToUpdate, dataset.Count, await entityServices.GetCount(), ValidationMilliseconds, stopwatch.ElapsedMilliseconds);
                }
                catch
                {
                    await EntityServices.RollbackTransaction(transaction).ConfigureAwait(false);
                    throw;
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/entities/bulk/5
        [HttpPost("bulk/{entityCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkEntitiesByEntityCode([FromRoute] string entityCode, [FromBody] IList<Entity> dataset, [FromQuery] bool deleteByEntityCode = false, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POSTBULK";
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                if (dataset is null)
                {
                    throw new ArgumentNullException(nameof(dataset));
                }
                HandleIncomingPayload(dataset);
                client = GetClient(client);
                var entityServices = new EntityServices(GetDBConfiguration(client));
                var entitiesToAdd = new List<Entity>();
                int entitiesToUpdate = 0;

                using var transaction = entityServices.BeginTransaction();
                try
                {
                    if (deleteByEntityCode)
                    {
                        await entityServices.DeleteByEntityCode(entityCode);
                        entitiesToAdd.AddRange(dataset);
                    }
                    else
                    {
                        List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                        string tableName = entityServices.GetTableName<Entity>();
                        Dictionary<string, string> uniqueIdentifierMapping = entityServices.GetColumnNames<Entity>(uniqueIdentifiers.ToArray());
                        foreach (Entity entity in dataset)
                        {
                            int id = await entityServices.CheckExistenceReturnId(entity, tableName, uniqueIdentifierMapping, null, transaction);
                            if (id > 0)
                            {
                                entitiesToUpdate++;
                                entity.ID = id;
                                await entityServices.UpdateEntityAsync(entity);
                            }
                            else
                            {
                                entitiesToAdd.Add(entity);
                            }
                        }
                    }
                    long ValidationMilliseconds = stopwatch.ElapsedMilliseconds;

                    await entityServices.CreateManyEntities(entitiesToAdd).ConfigureAwait(false);
                    await EntityServices.CommitTransaction(transaction).ConfigureAwait(false);
                    return FormatOkResponse(entitiesToAdd.Count, entitiesToUpdate, dataset.Count, await entityServices.GetCount(), ValidationMilliseconds, stopwatch.ElapsedMilliseconds);
                }
                catch
                {
                    await EntityServices.RollbackTransaction(transaction).ConfigureAwait(false);
                    throw;
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PUT: api/entities/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Entity), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutEntity([FromRoute] int id, [FromBody] Entity entity, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-PUT";
            try
            {
                if (entity is null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                if (id != entity.ID)
                {
                    string message = $"{methodName}: {nameof(id)} value does not matched with {nameof(entity.ID)} value.";
                    logger.Error(message);
                    return BadRequest(message);
                }
                client = GetClient(client);

                var entityServices = new EntityServices(GetDBConfiguration(client));
                BaseServices.UpdateModifiedDateTime(entity);
                logger.Debug($"{methodName}: Attempting to update entity record.", entity);
                await entityServices.UpdateEntityAsync(entity);
                return Ok(entity);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PATCH: api/entities/5
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Entity), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchEntity([FromRoute] int id, [FromBody] JsonPatchDocument<Entity> patch, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-PATCH";
            try
            {
                if (patch is null)
                {
                    throw new ArgumentNullException(nameof(patch));
                }
                client = GetClient(client);

                var entityServices = new EntityServices(GetDBConfiguration(client));
                Entity entity = await entityServices.GetOne(id).ConfigureAwait(false);
                if (entity == null)
                {
                    string message = $"{methodName}: No entity record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                patch.ApplyTo(entity);
                BaseServices.UpdateModifiedDateTime(entity);
                await entityServices.UpdateEntityAsync(entity);
                return Ok(entity);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // DELETE: api/entities/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEntity([FromRoute] int id, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-DELETE";

            try
            {
                client = GetClient(client);

                var entityServices = new EntityServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to fetch data from entityServices.GetEntityAsync() with {nameof(id)}: {id}");
                Entity entity = await entityServices.GetOne(id).ConfigureAwait(false);
                if (entity == null)
                {
                    string message = $"{methodName}: No entity record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                logger.Debug($"Entity record found for {nameof(id)}: {id}. Attempting to delete entity record.", entity);
                await entityServices.DeleteEntityAsync(entity);
                return NoContent();
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
    }
}