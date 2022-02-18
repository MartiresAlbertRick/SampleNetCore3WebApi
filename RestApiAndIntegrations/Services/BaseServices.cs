using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public class BaseServices
    {
        protected CAAPSDbContext Context { get; }
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static IConfiguration Configuration { get; private set; }

        public static void ConfigureService(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected BaseServices(DBConfiguration dbconfiguration)
        {
            if (dbconfiguration is null)
            {
                throw new ArgumentNullException(nameof(dbconfiguration));
            }
            Context = CAAPSDbContextFactory.Create(dbconfiguration);
        }

        protected BaseServices(CAAPSDbContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Checked")]
        public async Task<int> GetNewId(string storedProcedureName, DbConnection conn = null)
        {
            int result = 0;

            if (conn == null)
            {
                conn = Context.Database.GetDbConnection();
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
            }

            using (var command = conn.CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                if (Context.Database.CurrentTransaction.GetDbTransaction() != null)
                    command.Transaction = Context.Database.CurrentTransaction.GetDbTransaction();

                IDbDataParameter newIdParameter = command.CreateParameter();
                newIdParameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(newIdParameter);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                result = Utils.ParseObjectToInt(newIdParameter.Value);
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Checked")]
        public async Task<int[]> GetNewMultipleId(string storedProcedureName, int recordCount, DbConnection conn = null)
        {
            int[] result = new int[recordCount];

            if (conn == null)
            {
                conn = Context.Database.GetDbConnection();
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
            }

            using (var command = conn.CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                if (Context.Database.CurrentTransaction.GetDbTransaction() != null)
                    command.Transaction = Context.Database.CurrentTransaction.GetDbTransaction();

                IDbDataParameter newIdParameter = command.CreateParameter();
                newIdParameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(newIdParameter);

                for (int counter = 0; counter < recordCount; counter++)
                {
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result[counter] = Utils.ParseObjectToInt(newIdParameter.Value);
                }
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Checked")]
        public static async Task<int[]> GetNewMultipleId(string storedProcedureName, int recordCount, DbConnection connection, DbTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(storedProcedureName))
            {
                throw new ArgumentException($"No value set in argument {storedProcedureName}");
            }
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
                
            int[] result = new int[recordCount];

            using (var command = connection.CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                IDbDataParameter newIdParameter = command.CreateParameter();
                newIdParameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(newIdParameter);

                for (int counter = 0; counter < recordCount; counter++)
                {
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result[counter] = Utils.ParseObjectToInt(newIdParameter.Value);
                }
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Checked")]
        public async Task<int> CheckExistenceReturnId<T>(T entityType, string tableName, Dictionary<string, string> uniqueIdentifiers = null,
            DbConnection conn = null, IDbContextTransaction transaction = null)
        {
            var parameters = new Dictionary<string, object>();
            if (uniqueIdentifiers != null && uniqueIdentifiers.Count > 0)
            {
                if (entityType == null) throw new ArgumentNullException(nameof(entityType));
                if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"SELECT TOP 1 ID FROM {tableName} WHERE");
                int counter = 0;
                foreach (KeyValuePair<string, string> item in uniqueIdentifiers)
                {
                    PropertyInfo propInfo = entityType.GetType().GetProperty(item.Key);
                    object value = propInfo.GetValue(entityType);
                    string parameterName = $"@{item.Key}";
                    stringBuilder.Append($" {item.Value} = {parameterName} ");
                    parameters.Add(parameterName, value);
                    counter++;
                    if (uniqueIdentifiers.Count > counter)
                    {
                        stringBuilder.Append("AND");
                    }
                }
                if (conn == null)
                {
                    conn = Context.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        await conn.OpenAsync();
                }
                int result = 0;
                using (var command = conn.CreateCommand())
                {
                    if (transaction != null)
                    {
                        command.Transaction = transaction.GetDbTransaction();
                    }
                    command.CommandText = stringBuilder.ToString();
                    command.CommandType = CommandType.Text;
                    foreach (KeyValuePair<string, object> item in parameters)
                    {
                        IDbDataParameter parameter = command.CreateParameter();
                        parameter.ParameterName = item.Key;
                        parameter.Value = item.Value;
                        command.Parameters.Add(parameter);
                    }
                    result = Utils.ParseObjectToInt(await command.ExecuteScalarAsync().ConfigureAwait(false));
                }
                return result;
            }
            return 0;
        }

        public string GetTableName<T>()
        {
            IEntityType entityType = Context.Model.FindEntityType(typeof(T));
            string tableName = entityType.GetTableName(); // ? .Relational().TableName;
            if (string.IsNullOrWhiteSpace(tableName))
                throw new NotSupportedException($"No database table name found for entity {typeof(T).Name}");
            else
                return tableName;
        }

        public Dictionary<string, string> GetColumnNames<T>(string[] fieldNames)
        {
            if (fieldNames is null)
            {
                throw new ArgumentNullException(nameof(fieldNames));
            }

            var columnNames = new Dictionary<string, string>();
            foreach (string fieldName in fieldNames)
            {
                columnNames.Add(fieldName, GetColumnName<T>(fieldName));
            }
            return columnNames;
        }

        public string GetColumnName<T>(string fieldName)
        {
            IEntityType entityType = Context.Model.FindEntityType(typeof(T));
            return entityType.GetProperties().Where(t => t.Name == fieldName).FirstOrDefault().GetColumnName(); // ? .Relational().ColumnName;
        }

        const string LAST_MODIFIED_DATETIME = "LastModifiedDateTime";
        public static void UpdateModifiedDateTime<T>(T entityType)
        {
            SetPropertyValue(entityType, LAST_MODIFIED_DATETIME, DateTime.UtcNow);    
        }

        public static void UpdateModifiedDateTime<T>(IList<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            foreach (T obj in collection)
            {
                UpdateModifiedDateTime(obj);
            }
        }

        public static void SetPropertyValue<T>(T entityType, string propertyName, object value)
        {
            PropertyInfo propInfo = entityType.GetType().GetProperty(propertyName);
            if (propInfo != null)
                propInfo.SetValue(entityType, value);
        }

        public async Task<IList<T>> ImportAsync<T>(IList<T> dataset, string tableName, Dictionary<string, string> uniqueIdentifierMapping, bool truncateTable = false, bool generateSequence = false, string generateSequenceStoredProcedureName = null) where T : class
        {
            if (dataset is null)
            {
                throw new ArgumentNullException(nameof(dataset));
            }

            if (truncateTable)
                return await CreateRecordsAsync(dataset, truncateTable, generateSequence, generateSequenceStoredProcedureName).ConfigureAwait(false);

            var recordsToAdd = new List<T>();
            var recordsToUpdate = new List<T>();
            foreach (T obj in dataset)
            {
                int id = await CheckExistenceReturnId(obj, tableName, uniqueIdentifierMapping).ConfigureAwait(false);
                if (id > 0)
                {
                    SetPropertyValue(obj, ID, id);
                    recordsToUpdate.Add(obj);
                }
                else
                    recordsToAdd.Add(obj);
            }
            var result = new List<T>();
            result.AddRange(await CreateRecordsAsync(recordsToAdd, generateSequence: generateSequence, generateSequenceStoredProcedureName: generateSequenceStoredProcedureName).ConfigureAwait(false));
            result.AddRange(await UpdateManyRecordsAsync(recordsToUpdate).ConfigureAwait(false));
            return result;
        }

        const string ID = "ID";
        public async Task<IList<T>> CreateRecordsAsync<T>(IList<T> collection, bool truncateTable = false, bool generateSequence = false, string generateSequenceStoredProcedureName = null) where T : class
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            UpdateModifiedDateTime(collection);
            using (var conn = Context.Database.GetDbConnection())
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                using var transaction = Context.Database.BeginTransaction();
                if (truncateTable)
                {
                    string tableName = GetTableName<T>();
                    await Context.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}").ConfigureAwait(false);
                }
                if (generateSequence)
                {
                    if (string.IsNullOrWhiteSpace(generateSequenceStoredProcedureName))
                        throw new ArgumentNullException($"Argument {nameof(generateSequence)} is true but argument {nameof(generateSequenceStoredProcedureName)} is empty");
                    int[] ids = await GetNewMultipleId(generateSequenceStoredProcedureName, collection.Count, conn).ConfigureAwait(false);
                    int counter = 0;
                    foreach (T obj in collection)
                    {
                        SetPropertyValue(obj, ID, ids[counter]);
                        counter++;
                    }
                }
                await CreateManyRecords<T>(collection).ConfigureAwait(false);
                transaction.Commit();
            }
            return collection;
        }
        protected async Task CreateManyRecords<T>(IList<T> collection ) where T : class
        {
            if (Configuration != null && Configuration.GetValue<bool>("AppSettings:UseEFCoreBulkExtensions"))
            {
                logger.Trace("CreateManyRecords - using EFCore.BulkExtensions BulkInsertAsync");
                await Context.BulkInsertAsync<T>(collection).ConfigureAwait(false);
                logger.Trace("CreateManyRecords - BulkInsertAsync completed");
            }
            else
            {
                logger.Trace("CreateManyRecords - using EFCore.AddRangeAsync");
                await Context.AddRangeAsync(collection).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
                logger.Trace("CreateManyRecords - SaveChangesAsync completed");
            }
        }

        public async Task<IList<T>> UpdateManyRecordsAsync<T>(IList<T> collection) where T : class
        {
            if (Configuration != null && Configuration.GetValue<bool>("AppSettings:UseEFCoreBulkExtensions"))
            {
                await Context.BulkUpdateAsync<T>(collection).ConfigureAwait(false);
            }
            else
            {
                Context.UpdateRange(collection);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }
            return collection;
        }

        public async Task<IList<T>> UpdateRecordsWithModifiedDateTimeAsync<T>(IList<T> collection) where T : class
        {
            UpdateModifiedDateTime(collection);
            using (var transaction = Context.Database.BeginTransaction())
            {
                await UpdateManyRecordsAsync<T>(collection).ConfigureAwait(false);
                transaction.Commit();
            }
            return collection;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public static async Task CommitTransaction(IDbContextTransaction transaction)
        {
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            await transaction.CommitAsync();
        }

        public static async Task RollbackTransaction(IDbContextTransaction transaction)
        {
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            await transaction.RollbackAsync();
        }
    }
}