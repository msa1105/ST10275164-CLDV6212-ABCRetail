using ABCRetail.Functions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ABCRetail.Functions
{
    public class OrdersApi
    {
        private readonly ILogger<OrdersApi> _logger;
        private readonly TableClient _ordersTableClient;
        private readonly TableClient _customersTableClient;
        private readonly TableClient _productsTableClient;

        public OrdersApi(ILogger<OrdersApi> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

         
            _ordersTableClient = new TableClient(connectionString, "Order");
            _customersTableClient = new TableClient(connectionString, "Customer");
            _productsTableClient = new TableClient(connectionString, "Product");

            // This ensures the app doesn't crash if the tables don't exist yet,
            // but it will use existing tables if the names match.
            _ordersTableClient.CreateIfNotExists();
            _customersTableClient.CreateIfNotExists();
            _productsTableClient.CreateIfNotExists();
        }

        [Function("GetOrders")]
        public async Task<HttpResponseData> GetOrders(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders")] HttpRequestData req)
        {
            _logger.LogInformation("Request to get all orders.");

            try
            {
                var allOrders = await _ordersTableClient.QueryAsync<Order>().ToListAsync();
                var allCustomers = await _customersTableClient.QueryAsync<Customer>().ToListAsync();
                var allProducts = await _productsTableClient.QueryAsync<Product>().ToListAsync();

                var orderViewModels = from o in allOrders
                                      join c in allCustomers on o.CustomerId equals c.RowKey
                                      join p in allProducts on o.ProductId equals p.RowKey
                                      select new OrderViewModel
                                      {
                                          OrderId = o.RowKey,
                                          CustomerName = c.Name,
                                          ProductName = p.Name,
                                          TotalAmount = o.TotalAmount,
                                          OrderDate = o.OrderDate
                                      };

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(orderViewModels);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching orders.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred on the server.");
                return errorResponse;
            }
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

            var orderData = JsonSerializer.Deserialize<Order>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var order = new Order
            {
                PartitionKey = "order", // This can be anything, but "order" is a common convention
                RowKey = Guid.NewGuid().ToString(),
                CustomerId = orderData.CustomerId,
                ProductId = orderData.ProductId,
                TotalAmount = orderData.TotalAmount,
                OrderDate = DateTime.UtcNow, // Set the order date upon creation
                Timestamp = DateTime.UtcNow
            };

            await _ordersTableClient.AddEntityAsync(order);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(order);
            return response;
        }
    }
}

public static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items)
    {
        var results = new List<T>();
        await foreach (var item in items)
        {
            results.Add(item);
        }
        return results;
    }
}