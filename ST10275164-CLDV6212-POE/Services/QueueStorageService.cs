using Azure.Storage.Queues;

namespace ST10275164_CLDV6212_POE.Services
{
    public class QueueStorageService : IQueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;
        // Name of the queue for new order notifications
        private const string QueueName = "new-orders";

        public QueueStorageService(QueueServiceClient queueServiceClient)
        {
            _queueServiceClient = queueServiceClient;
        }

        public async Task SendMessageAsync(string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(QueueName);
            await queueClient.CreateIfNotExistsAsync();

            // Send the message
            await queueClient.SendMessageAsync(message);
        }
    }
}
