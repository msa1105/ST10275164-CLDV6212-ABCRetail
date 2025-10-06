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
    public class ProductsApi
    {
        private readonly ILogger<ProductsApi> _logger;
        private readonly TableClient _tableClient;

        public ProductsApi(ILogger<ProductsApi> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            _tableClient = new TableClient(connectionString, "product");
            _tableClient.CreateIfNotExists();
        }

        [Function("CreateProduct")]
        public async Task<HttpResponseData> CreateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
        {
            _logger.LogInformation("Request to create a product.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var product = JsonSerializer.Deserialize<Product>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                product.PartitionKey = "product";
                product.RowKey = Guid.NewGuid().ToString();

                _logger.LogInformation($"Attempting to add product with RowKey: {product.RowKey}");
                await _tableClient.AddEntityAsync(product);
                _logger.LogInformation("Product added successfully.");

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(product);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred on the server.");
                return errorResponse;
            }
        }

      

        [Function("GetProducts")]
        public async Task<HttpResponseData> GetProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
        {
            var products = new List<Product>();
            await foreach (var entity in _tableClient.QueryAsync<Product>())
            {
                products.Add(entity);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(products);
            return response;
        }

       
    }
}