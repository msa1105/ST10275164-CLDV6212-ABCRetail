using ABCRetail.Functions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ABCRetail.Functions
{
    public class OrdersApi
    {
        private readonly ILogger<OrdersApi> _logger;
        // Inject the main service client here since this API needs to access multiple tables
        private readonly TableServiceClient _tableServiceClient;

        public OrdersApi(ILogger<OrdersApi> logger, TableServiceClient tableServiceClient)
        {
            _logger = logger;
            _tableServiceClient = tableServiceClient;
        }

        [Function("CreateOrder")]
        public async Task<HttpResponseData> CreateOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] HttpRequestData req)
        {
            _logger.LogInformation("Request to create an order.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var newOrder = JsonSerializer.Deserialize<Order>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var orderTable = _tableServiceClient.GetTableClient("order");
                await orderTable.CreateIfNotExistsAsync();

                newOrder.PartitionKey = "order";
                newOrder.RowKey = Guid.NewGuid().ToString();
                newOrder.OrderDate = DateTime.UtcNow;

                _logger.LogInformation($"Attempting to add order with RowKey: {newOrder.RowKey}");
                await orderTable.AddEntityAsync(newOrder);
                _logger.LogInformation("Order added successfully.");

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(newOrder);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred on the server.");
                return errorResponse;
            }
        }

        [Function("GetOrders")]
        public async Task<HttpResponseData> GetOrders(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders")] HttpRequestData req)
        {
            var orderTable = _tableServiceClient.GetTableClient("order");
            var orders = new List<Order>();
            await foreach (var entity in orderTable.QueryAsync<Order>())
            {
                orders.Add(entity);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(orders);
            return response;
        }
    }
}