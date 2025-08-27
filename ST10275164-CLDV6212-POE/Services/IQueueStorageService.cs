namespace ST10275164_CLDV6212_POE.Services
{
    public interface IQueueStorageService
    {
        Task SendMessageAsync(string queueName, string message);
        Task<IEnumerable<string>> GetMessagesAsync(string queueName);
        Task<IEnumerable<string>> GetQueuesAsync();
    }
}