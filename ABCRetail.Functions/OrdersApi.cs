using ABCRetail.Functions.Models;
using Azure;
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

        public OrdersApi(ILogger<OrdersApi> logger)
        {
            _logger = logger;
        }

        [Function("GetOrders")]
        public async Task<HttpResponseData> GetOrders(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders")] HttpRequestData req)
        {
            _logger.LogInformation("Request for all orders.");
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var tableClient = new TableClient(connectionString, "Order");

            var orders = new List<Order>();
            await foreach (var entity in tableClient.QueryAsync<Order>())
            {
                orders.Add(entity);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(orders);
            return response;
        }

        [Function("CreateOrder")]
        public async Task<HttpResponseData> CreateOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] HttpRequestData req)
        {
            _logger.LogInformation("Request to create an order.");
            var newOrder = await JsonSerializer.DeserializeAsync<Order>(req.Body);

            if (newOrder == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var customerTable = new TableClient(connectionString, "customer");
            var productTable = new TableClient(connectionString, "product");
            var orderTable = new TableClient(connectionString, "order");

            // --- Business Logic ---
            // Get the full customer and product objects
            var customer = await customerTable.GetEntityAsync<Customer>("customer", newOrder.CustomerId);
            var product = await productTable.GetEntityAsync<Product>("product", newOrder.ProductId);

            // Populate the names and calculate the price on the server
            newOrder.CustomerId = customer.Value.Name;
            newOrder.ProductId = product.Value.Name;
            // --- End Business Logic ---

            newOrder.RowKey = Guid.NewGuid().ToString();
            newOrder.OrderDate = DateTime.UtcNow;

            await orderTable.AddEntityAsync(newOrder);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(newOrder);
            return response;
        }
    }
}