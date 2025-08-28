using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ST10275164_CLDV6212_POE.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly ILogger<TableStorageService> _logger;

        public TableStorageService(TableServiceClient tableServiceClient, ILogger<TableStorageService> logger)
        {
            _tableServiceClient = tableServiceClient;
            _logger = logger;
        }

        private async Task<TableClient> GetTableClient(string tableName) 
        {
            try
            {
                _logger.LogInformation($"Getting table client for: {tableName}");           //Logging for better error handling kindly showed to me by ChatGPT and implemented with help from
                var tableClient = _tableServiceClient.GetTableClient(tableName);            //this stack overflow article: https://stackoverflow.com/questions/7923085/handling-errors-and-exceptions-in-asp-net-mvc
                                                                                            //I plan on expanding the error catches etc. from table storage to a seperate page for admins later on
                _logger.LogInformation($"Creating table if not exists: {tableName}");           
                var response = await tableClient.CreateIfNotExistsAsync();

                if (response != null)
                {
                    _logger.LogInformation($"Table '{tableName}' was created");
                }
                else
                {
                    _logger.LogInformation($"Table '{tableName}' already exists");
                }

                return tableClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting/creating table client for: {tableName}");
                throw;
            }
        }

        public async Task<T> GetEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            try
            {
                var tableName = typeof(T).Name;
                _logger.LogInformation($"Getting entity from table '{tableName}' with PartitionKey='{partitionKey}' and RowKey='{rowKey}'");

                var tableClient = await GetTableClient(tableName);
                var response = await tableClient.GetEntityAsync<T>(partitionKey, rowKey);

                _logger.LogInformation($"Successfully retrieved entity from table '{tableName}'");
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting entity from table '{typeof(T).Name}' with PartitionKey='{partitionKey}' and RowKey='{rowKey}'");
                throw;
            }
        }

        public async Task<List<T>> GetAllEntitiesAsync<T>() where T : class, ITableEntity, new()
        {
            try
            {
                var tableName = typeof(T).Name;
                _logger.LogInformation($"Getting all entities from table: {tableName}");

                var tableClient = await GetTableClient(tableName);
                var entities = new List<T>();

                await foreach (var entity in tableClient.QueryAsync<T>())
                {
                    entities.Add(entity);
                }

                _logger.LogInformation($"Successfully retrieved {entities.Count} entities from table '{tableName}'");
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all entities from table '{typeof(T).Name}'");
                throw;
            }
        }

        public async Task<T> UpsertEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            try
            {
                var tableName = typeof(T).Name;
                _logger.LogInformation($"Upserting entity to table '{tableName}' with PartitionKey='{entity.PartitionKey}' and RowKey='{entity.RowKey}'");

                var tableClient = await GetTableClient(tableName);
                var response = await tableClient.UpsertEntityAsync(entity);

                _logger.LogInformation($"Successfully upserted entity to table '{tableName}'. Status: {response.Status}");
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error upserting entity to table '{typeof(T).Name}' with PartitionKey='{entity.PartitionKey}' and RowKey='{entity.RowKey}'");
                throw;
            }
        }

        public async Task DeleteEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            try
            {
                var tableName = typeof(T).Name;
                _logger.LogInformation($"Deleting entity from table '{tableName}' with PartitionKey='{partitionKey}' and RowKey='{rowKey}'");

                var tableClient = await GetTableClient(tableName);
                var response = await tableClient.DeleteEntityAsync(partitionKey, rowKey);

                _logger.LogInformation($"Successfully deleted entity from table '{tableName}'. Status: {response.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting entity from table '{typeof(T).Name}' with PartitionKey='{partitionKey}' and RowKey='{rowKey}'");
                throw;
            }
        }
    }
}