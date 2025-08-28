using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ST10275164_CLDV6212_POE.Services
{
    public class QueueStorageService : IQueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;

        public QueueStorageService(QueueServiceClient queueServiceClient)
        {
            _queueServiceClient = queueServiceClient;
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }

        public async Task<IEnumerable<string>> GetMessagesAsync(string queueName) // Get messages from queue
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName); // Get a reference to the queue
            var messages = new List<string>();

            if (await queueClient.ExistsAsync())
            {
                // PeekMessagesAsync returns a Response<PeekedMessage[]> object.
                var response = await queueClient.PeekMessagesAsync(maxMessages: 10);
                // !!!! This snippet was provided to me by ChatGPT to fix the issue of not displaying messages
                //and educate me on how to work with the PeekMessagesAsync method.
                //chatgpt.com
                // --- THE FIX IS HERE ---
                // We need to loop through response.Value, which contains the actual array of messages.
                foreach (PeekedMessage message in response.Value)
                {
                    messages.Add(message.Body.ToString());
                }
            }
            return messages; // Return the list of message bodies
        }

        public async Task<IEnumerable<string>> GetQueuesAsync()
        {
            var queueNames = new List<string>();
            await foreach (QueueItem queue in _queueServiceClient.GetQueuesAsync())
            {
                queueNames.Add(queue.Name); // Add each queue name to the list
            }
            return queueNames;
        }
    }
}