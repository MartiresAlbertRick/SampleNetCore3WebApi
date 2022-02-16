using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.Http;
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
    [Route("api/vendors")]
    [ApiController]
    public class VendorsController : BaseController
    {
        const string controllerName = "Vendors";

        // GET: api/vendors
        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ServiceFilter(typeof(CustomEnableQueryAttribute))]
        [ProducesResponseType(typeof(IQueryable<Vendor>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetVendors([FromQuery] string client = "") =>
            GetMany(MethodBase.GetCurrentMethod().Name, new VendorServices(GetDBConfiguration(GetClient(client))));

        // GET: api/vendors/5
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "DefaultCacheProfile")]
        [ProducesResponseType(typeof(Vendor), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetVendor([FromRoute] int id, [FromQuery] string client = "") =>
            await GetOne(MethodBase.GetCurrentMethod().Name, id, new VendorServices(GetDBConfiguration(GetClient(client)))).ConfigureAwait(false);

        // POST: api/vendors/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkVendors([FromBody] IList<Vendor> vendors, [FromQuery] bool truncateTable = false, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POSTBULK";
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                if (vendors is null)
                {
                    throw new ArgumentNullException(nameof(vendors));
                }
                HandleIncomingPayload(vendors);
                client = GetClient(client);
                LoadNormalizeFields(client, controllerName);
                foreach (Vendor vendor in vendors)
                {
                    NormalizeRecord(vendor);
                }

                var vendorServices = new VendorServices(GetDBConfiguration(client));
                using var transaction = vendorServices.BeginTransaction();
                try
                {
                    int vendorsToUpdate = 0;
                    var vendorsToAdd = new List<Vendor>();
                    if (truncateTable)
                    {
                        await vendorServices.DeleteAll().ConfigureAwait(false);
                        vendorsToAdd.AddRange(vendors);
                    }
                    else
                    {
                        List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                        string tableName = vendorServices.GetTableName<Vendor>();
                        Dictionary<string, string> uniqueIdentifierMapping = vendorServices.GetColumnNames<Vendor>(uniqueIdentifiers.ToArray());
                        foreach (Vendor vendor in vendors)
                        {
                            int id = await vendorServices.CheckExistenceReturnId(vendor, tableName, uniqueIdentifierMapping, null, transaction);
                            if (id > 0)
                            {
                                vendorsToUpdate++;
                                vendor.ID = id;
                                await vendorServices.UpdateVendor(vendor).ConfigureAwait(false);
                            }
                            else
                                vendorsToAdd.Add(vendor);
                        }
                    }
                    long validationMilliseconds = stopwatch.ElapsedMilliseconds;
                    await vendorServices.CreateManyVendors(vendorsToAdd).ConfigureAwait(false);
                    await VendorServices.CommitTransaction(transaction).ConfigureAwait(false);
                    return FormatOkResponse(vendorsToAdd.Count, vendorsToUpdate, vendors.Count, await vendorServices.GetCount(), validationMilliseconds, stopwatch.ElapsedMilliseconds);
                } catch
                {
                    await VendorServices.RollbackTransaction(transaction).ConfigureAwait(false);
                    throw;
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/vendors/bulk/5
        [HttpPost("bulk/{entityCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostBulkVendorsByEntityCode([FromRoute] string entityCode, [FromBody] IList<Vendor> dataset, [FromQuery] bool deleteByEntityCode = false, [FromQuery] string client = "")
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
                LoadNormalizeFields(client, controllerName);
                foreach (Vendor vendor in dataset)
                {
                    NormalizeRecord(vendor);
                }
                var vendorServices = new VendorServices(GetDBConfiguration(client));
                using var transaction = vendorServices.BeginTransaction();
                try
                {
                    var vendorsToAdd = new List<Vendor>();
                    int vendorsToUpdate = 0;
                    if (deleteByEntityCode)
                    {
                        await vendorServices.DeleteByEntityCode(entityCode).ConfigureAwait(false);
                        vendorsToAdd.AddRange(dataset);
                    }
                    else
                    {
                        List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                        string tableName = vendorServices.GetTableName<Vendor>();
                        Dictionary<string, string> uniqueIdentifierMapping = vendorServices.GetColumnNames<Vendor>(uniqueIdentifiers.ToArray());
                        foreach (Vendor vendor in dataset)
                        {
                            int id = await vendorServices.CheckExistenceReturnId(vendor, tableName, uniqueIdentifierMapping, null, transaction);
                            if (id > 0)
                            {
                                vendorsToUpdate++;
                                vendor.ID = id;
                                await vendorServices.UpdateVendor(vendor).ConfigureAwait(false);
                            }
                            else
                                vendorsToAdd.Add(vendor);
                        }
                    }
                    long validationMilliseconds = stopwatch.ElapsedMilliseconds;
                    await vendorServices.CreateManyVendors(vendorsToAdd).ConfigureAwait(false);
                    await VendorServices.CommitTransaction(transaction).ConfigureAwait(false);
                    return FormatOkResponse(vendorsToAdd.Count, vendorsToUpdate, dataset.Count, await vendorServices.GetCount(), validationMilliseconds, stopwatch.ElapsedMilliseconds);
                } catch
                {
                    await VendorServices.RollbackTransaction(transaction).ConfigureAwait(false);
                    throw;
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // POST: api/vendors
        [HttpPost]
        [ProducesResponseType(typeof(Vendor), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostVendor([FromBody] Vendor vendor, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-POST";
            try
            {
                if (vendor is null)
                {
                    throw new ArgumentNullException(nameof(vendor));
                }
                client = GetClient(client);
                LoadNormalizeFields(client, controllerName);
                NormalizeRecord(vendor);
                BaseServices.UpdateModifiedDateTime(vendor);
                var vendorServices = new VendorServices(GetDBConfiguration(client));
                List<string> uniqueIdentifiers = GetUniqueIdentifiers(client, controllerName);
                string tableName = vendorServices.GetTableName<Vendor>();
                Dictionary<string, string> uniqueIdentifierMapping = vendorServices.GetColumnNames<Vendor>(uniqueIdentifiers.ToArray());
                logger.Debug($"{methodName}: Attempting to create/update data.", vendor);
                int id = await vendorServices.CheckExistenceReturnId(vendor, tableName, uniqueIdentifierMapping);
                if (id > 0)
                {
                    vendor.ID = id;
                    await vendorServices.UpdateVendor(vendor);
                    return StatusCode(201, vendor);
                }
                else
                {
                    logger.Debug($"{methodName}: Vendor successfully created.", vendor);
                    return StatusCode(201, await vendorServices.CreateVendor(vendor));
                }
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PUT: api/vendors/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Vendor), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutVendor([FromRoute] int id, [FromBody] Vendor vendor, [FromQuery] string client = "")
        {
            
            string methodName = controllerName + "-PUT";
            try
            {
                if (vendor is null)
                {
                    throw new ArgumentNullException(nameof(vendor));
                }

                if (id != vendor.ID)
                {
                    string message = $"{methodName}: {nameof(id)} value does not matched with {nameof(vendor.ID)} value.";
                    logger.Error(message);
                    return BadRequest(message);
                }
                client = GetClient(client);
                LoadNormalizeFields(client, nameof(Vendor));
                NormalizeRecord(vendor);
                var vendorServices = new VendorServices(GetDBConfiguration(client));
                BaseServices.UpdateModifiedDateTime(vendor);
                logger.Debug($"{methodName}: Attempting to update vendor record.", vendor);
                await vendorServices.UpdateVendor(vendor);
                return Ok(vendor);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // PATCH: api/vendors/5
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Vendor), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchVendor([FromRoute] int id, [FromBody] JsonPatchDocument<Vendor> patch, [FromQuery] string client = "")
        {
            
            string methodName = controllerName + "-PATCH";
            try
            {
                if (patch is null)
                {
                    throw new ArgumentNullException(nameof(patch));
                }

                client = GetClient(client);

                var vendorServices = new VendorServices(GetDBConfiguration(client));
                Vendor vendor = await vendorServices.GetOne(id).ConfigureAwait(false);
                if (vendor == null)
                {
                    string message = $"{methodName}: No vendor record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                patch.ApplyTo(vendor);
                BaseServices.UpdateModifiedDateTime(vendor);
                await vendorServices.UpdateVendor(vendor);
                return Ok(vendor);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        // DELETE: api/vendors/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteVendor([FromRoute] int id, [FromQuery] string client = "")
        {
            string methodName = controllerName + "-DELETE";

            try
            {
                client = GetClient(client);

                var vendorServices = new VendorServices(GetDBConfiguration(client));
                logger.Debug($"{methodName}: Attempting to fetch data from vendorServices.GetVendors() with {nameof(id)}: {id}");
                Vendor vendor = await vendorServices.GetOne(id).ConfigureAwait(false);
                if (vendor == null)
                {
                    string message = $"{methodName}: No vendor record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                logger.Debug($"Vendor record found for {nameof(id)}: {id}. Attempting to delete vendor record.", vendor);
                await vendorServices.DeleteVendor(vendor);
                return NoContent();
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
    }
}