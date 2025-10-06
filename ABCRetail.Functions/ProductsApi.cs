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
            var product = await JsonSerializer.DeserializeAsync<Product>(req.Body);
            if (product != null)
            {
                // This line is the critical fix
                product.PartitionKey = "product";
                product.RowKey = Guid.NewGuid().ToString();
                await _tableClient.AddEntityAsync(product);

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(product);
                return response;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        // --- Other functions (GetProducts, Update, etc.) remain below ---

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

        [Function("GetProductById")]
        public async Task<HttpResponseData> GetProductById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpRequestData req,
            string id)
        {
            var product = await _tableClient.GetEntityAsync<Product>("product", id);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(product.Value);
            return response;
        }

        [Function("UpdateProduct")]
        public async Task<HttpResponseData> UpdateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id}")] HttpRequestData req,
            string id)
        {
            var updatedProduct = await JsonSerializer.DeserializeAsync<Product>(req.Body);
            if (updatedProduct != null)
            {
                await _tableClient.UpdateEntityAsync(updatedProduct, ETag.All, TableUpdateMode.Replace);
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(updatedProduct);
                return response;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        [Function("DeleteProduct")]
        public async Task<HttpResponseData> DeleteProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id}")] HttpRequestData req,
            string id)
        {
            await _tableClient.DeleteEntityAsync("product", id);
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}