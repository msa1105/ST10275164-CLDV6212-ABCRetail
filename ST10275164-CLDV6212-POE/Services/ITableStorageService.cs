using Azure.Data.Tables;

namespace ST10275164_CLDV6212_POE.Services
{
    public interface ITableStorageService
    {
        Task<T> GetEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new();         //get single entity
        Task<List<T>> GetAllEntitiesAsync<T>() where T : class, ITableEntity, new();                                //get all entities
        Task<T> UpsertEntityAsync<T>(T entity) where T : class, ITableEntity;                                       //insert or update entity
        Task DeleteEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new();         //delete entity (will be implemented in admin moves)
    }
}
