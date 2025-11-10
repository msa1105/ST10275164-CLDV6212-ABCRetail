using ABCRetail.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore; // <-- Add this
using System.IO;                   // <-- Add this
using System.Threading.Tasks;      // <-- Add this
using System.Collections.Generic;    // <-- Add this
using System.Linq;                 // <-- Add this

namespace ABCRetail.Functions
{
    public class ProductsApi
    {
        private readonly ILogger<ProductsApi> _logger;
        private readonly AppDbContext _context; // <-- This is the NEW SQL connection

        // Inject the AppDbContext (SQL) instead of the TableClient
        public ProductsApi(ILogger<ProductsApi> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context; // <-- Assign the SQL context
        }

        [Function("CreateProduct")]
        public async Task<HttpResponseData> CreateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
        {
            _logger.LogInformation("Request to create a product (SQL).");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                // This 'Product' model has 'int ProductId'
                var product = JsonSerializer.Deserialize<Product>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _context.Products.Add(product);
                await _context.SaveChangesAsync(); // <-- Saves to SQL

                _logger.LogInformation($"Product added to SQL successfully with ID: {product.ProductId}");

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(product);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product (SQL).");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred on the server.");
                return errorResponse;
            }
        }

        [Function("GetProducts")]
        public async Task<HttpResponseData> GetProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
        {
            _logger.LogInformation("Request to get all products (SQL).");

            // --- THIS IS THE FIX ---
            // This pulls from the SQL database, which has the correct 'int ProductId'
            var products = await _context.Products.ToListAsync();
            // --- END OF FIX ---

            var response = req.CreateResponse(HttpStatusCode.OK);
            // This will now send JSON with {"productId": 1} (a number)
            await response.WriteAsJsonAsync(products);
            return response;
        }

        // --- ADDED MISSING FUNCTIONS THAT YOUR WEB APP NEEDS ---
        // Your ProductsController.cs needs these for Edit/Delete pages

        [Function("GetProduct")]
        public async Task<HttpResponseData> GetProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"Request to get product by ID: {id}");
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(product);
            return response;
        }

        [Function("UpdateProduct")]
        public async Task<HttpResponseData> UpdateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"Request to update product by ID: {id}");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var productData = JsonSerializer.Deserialize<Product>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (id != productData.ProductId)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            _context.Entry(productData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.ProductId == id))
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    throw;
                }
            }
            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        [Function("DeleteProduct")]
        public async Task<HttpResponseData> DeleteProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"Request to delete product by ID: {id}");
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}