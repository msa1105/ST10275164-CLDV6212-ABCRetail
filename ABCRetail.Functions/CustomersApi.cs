using ABCRetail.Functions.Models;
// using Azure; // <-- No longer needed
// using Azure.Data.Tables; // <-- No longer needed
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore; // <-- ADD THIS
using System.IO;                   // <-- ADD THIS
using System.Threading.Tasks;      // <-- ADD THIS
using System.Collections.Generic;    // <-- ADD THIS

namespace ABCRetail.Functions
{
    public class CustomersApi
    {
        private readonly ILogger<CustomersApi> _logger;
        private readonly AppDbContext _context; // <-- CHANGE THIS

        // Inject the AppDbContext here
        public CustomersApi(ILogger<CustomersApi> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context; // <-- ASSIGN THIS
            // All TableClient logic is removed
        }

        [Function("CreateCustomer")]
        public async Task<HttpResponseData> CreateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
        {
            _logger.LogInformation("Request to create a customer (SQL).");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var customer = JsonSerializer.Deserialize<Customer>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // --- NEW EF Core Logic ---
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                // --- End EF Core Logic ---

                _logger.LogInformation($"Customer added to SQL successfully with ID: {customer.CustomerId}");

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(customer);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the customer (SQL).");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred on the server.");
                return errorResponse;
            }
        }

        [Function("GetCustomers")]
        public async Task<HttpResponseData> GetCustomers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
        {
            _logger.LogInformation("Request to get all customers (SQL).");

            // --- NEW EF Core Logic ---
            var customers = await _context.Customers.ToListAsync();
            // --- End EF Core Logic ---

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers);
            return response;
        }
    }
}