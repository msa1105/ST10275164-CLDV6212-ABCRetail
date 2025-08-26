using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ST10275164_CLDV6212_POE.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableServiceClient _tableServiceClient;

        public TableStorageService(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        private async Task<TableClient> GetTableClient(string tableName)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        public async Task<T> GetEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            var tableName = typeof(T).Name;
            var tableClient = await GetTableClient(tableName);
            return await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
        }

        public async Task<List<T>> GetAllEntitiesAsync<T>() where T : class, ITableEntity, new()
        {
            var tableName = typeof(T).Name;
            var tableClient = await GetTableClient(tableName);
            var entities = new List<T>();
            await foreach (var entity in tableClient.QueryAsync<T>())
            {
                entities.Add(entity);
            }
            return entities;
        }

        public async Task<T> UpsertEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            var tableName = typeof(T).Name;
            var tableClient = await GetTableClient(tableName);
            await tableClient.UpsertEntityAsync(entity);
            return entity;
        }

        public async Task DeleteEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            var tableName = typeof(T).Name;
            var tableClient = await GetTableClient(tableName);
            await tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}
