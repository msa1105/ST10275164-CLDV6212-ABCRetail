namespace ST10275164_CLDV6212_POE.Services
{
    public interface IQueueStorageService
    {
        Task SendMessageAsync(string queueName, string message);            //send message to queue
        Task<IEnumerable<string>> GetMessagesAsync(string queueName);       //get messages from queue
        Task<IEnumerable<string>> GetQueuesAsync();                         //list of queues
    }
}