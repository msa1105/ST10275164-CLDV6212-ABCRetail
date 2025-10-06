using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ABCRetail.Functions
{
    public class QueuesApi
    {
        private readonly ILogger<QueuesApi> _logger;
        private readonly QueueServiceClient _queueServiceClient;

        public QueuesApi(ILogger<QueuesApi> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            _queueServiceClient = new QueueServiceClient(connectionString);
        }

        [Function("GetQueues")]
        public async Task<HttpResponseData> GetQueues(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "queues")] HttpRequestData req)
        {
            var queueNames = new List<string>();
            await foreach (var queue in _queueServiceClient.GetQueuesAsync())
            {
                queueNames.Add(queue.Name);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(queueNames);
            return response;
        }

        [Function("CreateQueue")]
        public async Task<HttpResponseData> CreateQueue(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "queues/{queueName}")] HttpRequestData req, string queueName)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            return req.CreateResponse(HttpStatusCode.Created);
        }

        [Function("GetQueueMessages")]
        public async Task<HttpResponseData> GetQueueMessages(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "queues/{queueName}/messages")] HttpRequestData req, string queueName)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            var messagesText = new List<string>();
            if (await queueClient.ExistsAsync())
            {
                var messages = await queueClient.PeekMessagesAsync(10);
                foreach (var msg in messages.Value)
                {
                    messagesText.Add(msg.MessageText);
                }
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(messagesText);
            return response;
        }

        [Function("AddQueueMessage")]
        public async Task<HttpResponseData> AddQueueMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "queues/{queueName}/messages")] HttpRequestData req, string queueName)
        {
            var messageContent = await req.ReadAsStringAsync();
            if (string.IsNullOrEmpty(messageContent)) return req.CreateResponse(HttpStatusCode.BadRequest);

            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(messageContent);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}