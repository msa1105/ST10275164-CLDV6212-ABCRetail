//This entire page was provided to me by ChatGPT to test my project without manually entering information and entities 
//chatgpt.com

using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Services;
using ST10275164_CLDV6212_POE.Models;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class DiagnosticController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueStorageService _queueStorageService;
        private readonly IFileStorageService _fileStorageService;
        private readonly TableServiceClient _tableServiceClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;
        private readonly ShareServiceClient _shareServiceClient;
        private readonly ILogger<DiagnosticController> _logger;

        public DiagnosticController(
            ITableStorageService tableStorageService,
            IBlobStorageService blobStorageService,
            IQueueStorageService queueStorageService,
            IFileStorageService fileStorageService,
            TableServiceClient tableServiceClient,
            BlobServiceClient blobServiceClient,
            QueueServiceClient queueServiceClient,
            ShareServiceClient shareServiceClient,
            ILogger<DiagnosticController> logger)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _queueStorageService = queueStorageService;
            _fileStorageService = fileStorageService;
            _tableServiceClient = tableServiceClient;
            _blobServiceClient = blobServiceClient;
            _queueServiceClient = queueServiceClient;
            _shareServiceClient = shareServiceClient;
            _logger = logger;
        }

        public async Task<IActionResult> TestConnection()
        {
            var results = new List<string>();

            try
            {
                
                results.Add("=== TABLE STORAGE TEST ===");
                await foreach (var table in _tableServiceClient.QueryAsync())
                {
                    results.Add($"Found table: {table.Name}");
                }

                
                var testCustomer = new Customer
                {
                    CustomerId = Guid.NewGuid().ToString(),
                    PartitionKey = "Customer",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "Test Customer " + DateTime.Now.ToString("HH:mm:ss"),
                    Email = "test@example.com"
                };

                await _tableStorageService.UpsertEntityAsync(testCustomer);
                results.Add($"✅ Successfully created test customer: {testCustomer.Name}");

                
                var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
                results.Add($"✅ Retrieved {customers.Count} customers from storage");

                foreach (var customer in customers.Take(5)) 
                {
                    results.Add($"  - Customer: {customer.Name} ({customer.Email})");
                }
            }
            catch (Exception ex)
            {
                results.Add($"❌ Table Storage Error: {ex.Message}");
                _logger.LogError(ex, "Table Storage test failed");
            }

            try
            {
                
                results.Add("\n=== BLOB STORAGE TEST ===");
                await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
                {
                    results.Add($"Found container: {container.Name}");
                }

                
                var testContent = "This is a test file created at " + DateTime.Now;
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testContent));
                var fileName = $"test-{DateTime.Now:yyyyMMddHHmmss}.txt";
                var blobUrl = await _blobStorageService.UploadFileToBlobAsync(fileName, stream);
                results.Add($"✅ Successfully uploaded test blob: {fileName}");
            }
            catch (Exception ex)
            {
                results.Add($"❌ Blob Storage Error: {ex.Message}");
                _logger.LogError(ex, "Blob Storage test failed");
            }

            try
            {
                // Test Queue Storage Connection
                results.Add("\n=== QUEUE STORAGE TEST ===");
                await foreach (var queue in _queueServiceClient.GetQueuesAsync())
                {
                    results.Add($"Found queue: {queue.Name}");
                }

               // await _queueStorageService.SendMessageAsync($"Test message at {DateTime.Now}");
                results.Add("✅ Successfully sent test queue message");
            }
            catch (Exception ex)
            {
                results.Add($"❌ Queue Storage Error: {ex.Message}");
                _logger.LogError(ex, "Queue Storage test failed");
            }

            try
            {
                // Test File Storage Connection
                results.Add("\n=== FILE STORAGE TEST ===");
                await foreach (var share in _shareServiceClient.GetSharesAsync())
                {
                    results.Add($"Found share: {share.Name}");
                }

                var testFileContent = "Test file content at " + DateTime.Now;
                using var fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testFileContent));
                await _fileStorageService.UploadFileAsync($"test-{DateTime.Now:yyyyMMddHHmmss}.txt", fileStream);
                results.Add("✅ Successfully uploaded test file");
            }
            catch (Exception ex)
            {
                results.Add($"❌ File Storage Error: {ex.Message}");
                _logger.LogError(ex, "File Storage test failed");
            }

            // Test configuration
            results.Add("\n=== CONFIGURATION TEST ===");
            results.Add($"Storage Account Name: {_blobServiceClient.AccountName}");
            results.Add($"Blob Service URI: {_blobServiceClient.Uri}");
            results.Add($"Table Service URI: {_tableServiceClient.Uri}");

            ViewBag.Results = results;
            return View();
        }

        public async Task<IActionResult> TestProducts()
        {
            var results = new List<string>();

            try
            {
                // Test creating a product without image
                results.Add("=== TESTING PRODUCT CREATION (No Image) ===");
                var testProduct1 = new Product
                {
                    ProductId = Guid.NewGuid().ToString(),
                    PartitionKey = "Product",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "Test Product " + DateTime.Now.ToString("HH:mm:ss"),
                    Description = "Test product description",
                    Price = 19.99
                };

                await _tableStorageService.UpsertEntityAsync(testProduct1);
                results.Add($"✅ Successfully created test product (no image): {testProduct1.Name}");

                // Test creating a product with image
                results.Add("\n=== TESTING PRODUCT CREATION (With Image) ===");
                var testProduct2 = new Product
                {
                    ProductId = Guid.NewGuid().ToString(),
                    PartitionKey = "Product",
                    RowKey = Guid.NewGuid().ToString(),
                    Name = "Test Product with Image " + DateTime.Now.ToString("HH:mm:ss"),
                    Description = "Test product with image description",
                    Price = 29.99
                };

                // Create a test image (small PNG)
                var testImageData = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");
                using var imageStream = new MemoryStream(testImageData);
                var testFileName = $"test-product-{DateTime.Now:yyyyMMddHHmmss}.png";

                await _blobStorageService.UploadFileToBlobAsync(testFileName, imageStream);
                testProduct2.ImageUrl = testFileName;

                await _tableStorageService.UpsertEntityAsync(testProduct2);
                results.Add($"✅ Successfully created test product (with image): {testProduct2.Name}");
                results.Add($"✅ Image stored as: {testFileName}");

                // Retrieve all products
                var allProducts = await _tableStorageService.GetAllEntitiesAsync<Product>();
                results.Add($"\n✅ Total products in storage: {allProducts.Count}");

                foreach (var product in allProducts.Take(5))
                {
                    results.Add($"  - Product: {product.Name} (${product.Price}) - Image: {product.ImageUrl ?? "None"}");
                }

                // Test ModelState validation
                results.Add("\n=== TESTING MODEL VALIDATION ===");
                var invalidProduct = new Product(); // Empty product
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(invalidProduct);
                var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(invalidProduct, validationContext, validationResults, true);

                results.Add($"Empty product validation result: {(isValid ? "Valid" : "Invalid")}");
                foreach (var error in validationResults)
                {
                    results.Add($"  - Validation Error: {error.ErrorMessage}");
                }

                // Test valid product validation
                var validProduct = new Product
                {
                    Name = "Valid Product",
                    Description = "Valid Description",
                    Price = 10.00
                };

                validationResults.Clear();
                validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(validProduct);
                isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(validProduct, validationContext, validationResults, true);

                results.Add($"Valid product validation result: {(isValid ? "Valid" : "Invalid")}");
                foreach (var error in validationResults)
                {
                    results.Add($"  - Validation Error: {error.ErrorMessage}");
                }

            }
            catch (Exception ex)
            {
                results.Add($"❌ Error during product testing: {ex.Message}");
                results.Add($"Stack Trace: {ex.StackTrace}");
                _logger.LogError(ex, "Product testing failed");
            }

            ViewBag.Results = results;
            return View("TestConnection");
        }

        public async Task<IActionResult> ClearTestData()
        {
            var results = new List<string>();

            try
            {
                // Delete test customers
                var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
                var testCustomers = customers.Where(c => c.Name.StartsWith("Test Customer")).ToList();

                foreach (var customer in testCustomers)
                {
                    await _tableStorageService.DeleteEntityAsync<Customer>(customer.PartitionKey, customer.RowKey);
                    results.Add($"Deleted test customer: {customer.Name}");
                }

                results.Add($"✅ Cleaned up {testCustomers.Count} test customers");
            }
            catch (Exception ex)
            {
                results.Add($"❌ Error cleaning test data: {ex.Message}");
                _logger.LogError(ex, "Error cleaning test data");
            }

            ViewBag.Results = results;
            return View("TestConnection");
        }
    }
}