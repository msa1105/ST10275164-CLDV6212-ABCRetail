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

        [Function("CreateCustomer")]
        public async Task<HttpResponseData> CreateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
        {
            _logger.LogInformation("Request to create a customer.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var customer = JsonSerializer.Deserialize<Customer>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                customer.PartitionKey = "customer";
                customer.RowKey = Guid.NewGuid().ToString();

                _logger.LogInformation($"Attempting to add customer with RowKey: {customer.RowKey}");
                await _tableClient.AddEntityAsync(customer);
                _logger.LogInformation("Customer added successfully.");

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(customer);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the customer.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred on the server.");
                return errorResponse;
            }
        }

        // --- All other functions (GetCustomers, etc.) remain below ---

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

        // ... (Include the rest of your GetById, Update, and Delete functions here)
    }
}