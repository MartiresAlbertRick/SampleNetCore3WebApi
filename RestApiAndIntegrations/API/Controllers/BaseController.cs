using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Entities.CustomExceptions;
using AD.CAAPS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace AD.CAAPS.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected static IConfiguration Configuration { get; private set; }

        public static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void ConfigureControllers(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected BaseController()
        {
        }

        [NonAction]
        protected string GetClient(string client) => string.IsNullOrWhiteSpace(client) ? GetClientIdFromRequestHeaders() : client;

        [NonAction]
        protected OkObjectResult FormatOkResponse(int CreatedCount, int UpdatedCount, int TotalCount, int TableCount, 
            long ValidationMilliseconds, long TotalRequestMilliseconds)
        {
            string Message = $"{CreatedCount}/{TotalCount} records created. {UpdatedCount:N0}/{TotalCount:N0} records updated. Time elapsed: {TotalRequestMilliseconds:N0}ms";
            logger.Debug(Message);
            decimal RecordsPerSecond = 0.0M;
            if (TotalRequestMilliseconds > 0)
            {
                RecordsPerSecond = Math.Round((decimal)TotalCount / (decimal)TotalRequestMilliseconds * 1000.0M, 2);
            };
            return Ok(new
            {
                Message,
                CreatedCount,
                UpdatedCount,
                TotalCount,
                TableCount,
                ValidationMilliseconds,
                TotalRequestMilliseconds,
                RecordsPerSecond
            });
        }


        [NonAction]
        protected string GetClientIdFromRequestHeaders()
        {
            const string clientId = "x-client-id";
            logger.Debug($"Attempting to retrieve {clientId} value from request headers.");
            string client = Request.Headers[clientId];
            if (string.IsNullOrWhiteSpace(client))
                throw new CaapsApiClientRequestException($"Could not find {clientId} from the request headers.", new System.Net.Http.HttpRequestException());
            logger.Debug($"Successfully retrieved {clientId} from request headers with value: {client}");
            return client;
        }

        [NonAction]
        protected static string GetClientConnectionString(string client)
        {
            if (string.IsNullOrWhiteSpace(client))
                throw new CaapsApiClientRequestException($"No value assigned for a required argument - {nameof(client)}", new ArgumentNullException(nameof(client)));
            if (Configuration == null)
                throw new CaapsApiInternalException($"{nameof(Configuration)} not found", new NullReferenceException(nameof(Configuration)));
            logger.Debug($"Attempting to retrieve database connection string for client {client}");
            string connectionString = Configuration["ConnectionStrings:" + client];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new CaapsApiInternalException($"Missing configuration for client: {client}. Could not find connection string.", new NullReferenceException(nameof(connectionString)));
            logger.Debug($"Successfully retrieved connection string for client {client}. Connection string info: {connectionString}");
            return connectionString;
        }

        [NonAction]
        public IActionResult ConvertExceptionToHttpStatusCode(Exception exception, string methodName)
        {
            if (exception == null)
                return StatusCode(500, "Caught an exception but the exception argument is null");
            string errorMessage = exception.Message;
            if (Startup.IsDevelopment)
                errorMessage = exception.ToString();
            logger.Error(exception, $"An exception caught while running {methodName}. {errorMessage}");
            if (exception is CaapsApiClientRequestException)
                return BadRequest(errorMessage);
            else
                return StatusCode(500, errorMessage);
        }

        [NonAction]
        public static DBConfiguration GetDBConfiguration(string client)
        {
            return new DBConfiguration
            {
                ConnectionString = GetClientConnectionString(client),
                DateFormat = Configuration["AppSettings:DateFormat"]
            };
        }

        public List<string> NormalizeFields { get; } = new List<string>();
        public List<string> LeftPadWithZero { get; } = new List<string>();

        [NonAction]
        public void LoadNormalizeFields(string client, string tableName)
        {
            var collection = new Dictionary<string, Dictionary<string, bool>>();

            string configKey = string.Format("AppSettings:ClientSettings:{0}:FieldSettings:{1}", client, tableName);
            Configuration.GetSection(configKey).Bind(collection);

            foreach (var key in collection.Keys)
            {
                // add fields with value == true
                if (collection[key].ContainsKey("NormalizeOnUpdate") && collection[key]["NormalizeOnUpdate"])
                    NormalizeFields.Add(key);
                if (collection[key].ContainsKey("LeftPadWithZero") && collection[key]["LeftPadWithZero"])
                    LeftPadWithZero.Add(key);
            }
        }

        [NonAction]
        protected static Amazon.S3.IAmazonS3 InitializeAmazonS3Client()
        {
            AWSS3Credentials s3Credentials = GetS3Credentials();
            return AmazonS3Utils.CreateClient(s3Credentials.AWSAccessKeyId, s3Credentials.AWSSecretAccessKey);
        }

        [NonAction]
        private static AWSS3Credentials GetS3Credentials()
        {
            var s3credentials = new AWSS3Credentials();
            Configuration.GetSection("AppSettings:AWSS3Credentials").Bind(s3credentials);

            if (string.IsNullOrWhiteSpace(s3credentials.AWSAccessKeyId))
                throw new NullReferenceException($"Check configuration. Value is required for {nameof(s3credentials.AWSAccessKeyId)}");
            if (string.IsNullOrWhiteSpace(s3credentials.AWSSecretAccessKey))
                throw new NullReferenceException($"Check configuration. Value is required for {nameof(s3credentials.AWSSecretAccessKey)}");

            return s3credentials;
        }

        [NonAction]
        public void NormalizeRecord<T>(T rec)
        {
            foreach (var field in NormalizeFields)
            {
                var propInfo = rec.GetType().GetProperty(field);

                if (propInfo != null)
                {
                    if (propInfo.PropertyType == typeof(string))
                    {
                        string value = Utils.ConvertObjectToString(propInfo.GetValue(rec));

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            value = Utils.CleanString(value);
                            value = IsFieldVBNThenNormalize(propInfo.Name, value);
                            propInfo.SetValue(rec, value);
                        }
                    }
                }
            }
        }

        [NonAction]
        public string IsFieldVBNThenNormalize(string propertyName, string value)
        {
            if (propertyName == "VendorBusinessNumber" && LeftPadWithZero.Contains(propertyName))
            {
                value = Utils.CleanString(value);

                if (Utils.IsDigit(value))
                {
                    int length = value.Length;

                    if (length >= 11)
                        return value;
                    if (length == 9)
                        return value;
                    if (length > 9 && length < 11)
                        return value.PadLeft(11, '0');
                    if (length >= 7 && length < 9)
                        return value.PadLeft(9, '0');
                }
            }
            return value;
        }

        [NonAction]
        public static List<string> GetUniqueIdentifiers(string client, string tableName)
        {
            var collection = new List<string>();
            string configKey = string.Format("AppSettings:ClientSettings:{0}:UniqueIdentifiers:{1}", client, tableName);
            Configuration.GetSection(configKey).Bind(collection);
            return collection;
        }

        [NonAction]
        public IActionResult GetMany<T>(string methodName, IQueryable<T> queryResult)
        {
            try
            {
                logger.Info($"{methodName}: Attempting to fetch data.");
                return Ok(queryResult);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        [NonAction]
        public IActionResult GetMany<T>(string methodName, IReadOnlyService<T> service)
        {
            try
            {
                if (service == null) throw new CaapsApiInternalException($"Missing value for argument {nameof(service)}", new ArgumentNullException(nameof(service)));
                logger.Info($"{methodName}: Attempting to fetch data.");
                return Ok(service.GetMany());
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        [NonAction]
        public async Task<IActionResult> GetOne<TEntity>(string methodName, int id, IReadOnlyService<TEntity> service) where TEntity : class
        {
            try
            {
                if (service == null) throw new CaapsApiInternalException($"Missing value for argument {nameof(service)}", new ArgumentNullException(nameof(service)));
                logger.Info($"{methodName}: Attempting to fetch with {nameof(id)}: {id}");
                TEntity queryResult = await service.GetOne(id).ConfigureAwait(false);
                if (queryResult == null)
                {
                    string message = $"{methodName}: No record found for {nameof(id)}: {id}";
                    logger.Error(message);
                    return NotFound(message);
                }
                logger.Info($"{methodName}: Record found for {nameof(id)}: {id}");
                return Ok(queryResult);
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }
        

        [NonAction]
        public async Task<IActionResult> Create<T>(string methodName, string client, IList<T> dataset, IService<T> service, bool truncateTable = false)
        {
            try
            {
                if (service == null) throw new CaapsApiInternalException($"Missing value for argument {nameof(service)}", new ArgumentNullException(nameof(service)));
                return StatusCode(201, await service.Import(dataset,
                                                            GetUniqueIdentifiers(client, typeof(T).Name).ToArray(),
                                                            truncateTable).ConfigureAwait(false));
            }
            catch (Exception exception)
            {
                return ConvertExceptionToHttpStatusCode(exception, methodName);
            }
        }

        [NonAction]
        public static void HandleIncomingPayload<T>(IList<T> objects)
        {
            BaseServices.UpdateModifiedDateTime(objects);
            logger.Debug($"Received payload: {JsonConvert.SerializeObject(objects)}");
        }
    }
}