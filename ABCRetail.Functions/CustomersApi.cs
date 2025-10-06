using ABCRetail.Functions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ABCRetail.Functions
{
    public class CustomersApi
    {
        private readonly ILogger<CustomersApi> _logger;
        private readonly TableClient _tableClient;

        public CustomersApi(ILogger<CustomersApi> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            _tableClient = new TableClient(connectionString, "customer");
            _tableClient.CreateIfNotExists();
        }

        [Function("GetCustomers")]
        public async Task<HttpResponseData> GetCustomers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
        {
            var customers = new List<Customer>();
            await foreach (var entity in _tableClient.QueryAsync<Customer>())
            {
                customers.Add(entity);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers);
            return response;
        }

        [Function("CreateCustomer")]
        public async Task<HttpResponseData> CreateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
        {
            _logger.LogInformation("Request to create a customer.");
            var customer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);
            if (customer != null)
            {
                // This line is the critical fix
                customer.PartitionKey = "customer";
                customer.RowKey = Guid.NewGuid().ToString();
                await _tableClient.AddEntityAsync(customer);

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(customer);
                return response;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}