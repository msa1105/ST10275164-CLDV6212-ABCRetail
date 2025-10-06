using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProductsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            var functionApiUrl = configuration["FunctionApiUrl"];
            if (string.IsNullOrEmpty(functionApiUrl))
            {
                throw new InvalidOperationException("The 'FunctionApiUrl' configuration setting is missing from appsettings.json.");
            }
            _apiUrl = $"{functionApiUrl.TrimEnd('/')}/products";

            // Initialize Blob Service Client
            var connectionString = configuration.GetSection("AzureStorage")["ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var products = await client.GetFromJsonAsync<IEnumerable<Product>>(_apiUrl, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(products ?? new List<Product>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                TempData["Error"] = "Could not load products. Please ensure the Function App is running.";
                return View(new List<Product>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Description")] Product product, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle image upload if provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient("product-images");
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        var blobName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                        var blobClient = containerClient.GetBlobClient(blobName);

                        using (var stream = imageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = imageFile.ContentType });
                        }

                        product.ImageUrl = blobClient.Uri.ToString();
                    }

                    var client = _httpClientFactory.CreateClient();
                    var response = await client.PostAsJsonAsync(_apiUrl, product);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Product created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Failed to create product. Status: {response.StatusCode}, Error: {errorContent}");
                        ModelState.AddModelError("", $"Failed to create product: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError("", "An error occurred while creating the product. Please ensure the Function App is running.");
            }

            return View(product);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var product = await client.GetFromJsonAsync<Product>($"{_apiUrl}/{id}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product {id}");
                TempData["Error"] = "Could not load product.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Price,Description,ImageUrl,PartitionKey,RowKey,Timestamp,ETag")] Product product)
        {
            if (id != product.RowKey) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.PutAsJsonAsync($"{_apiUrl}/{id}", product);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update product.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product");
                    ModelState.AddModelError("", "An error occurred while updating the product.");
                }
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var product = await client.GetFromJsonAsync<Product>($"{_apiUrl}/{id}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product {id} for deletion");
                TempData["Error"] = "Could not load product.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                await client.DeleteAsync($"{_apiUrl}/{id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                TempData["Error"] = "Could not delete product.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}