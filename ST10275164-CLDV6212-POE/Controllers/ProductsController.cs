using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs; // Add this
using System.IO;
using System.Threading.Tasks;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;
        private readonly BlobServiceClient _blobServiceClient; // Add Blob Service Client

        // Inject BlobServiceClient into the constructor
        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["FunctionApiUrl"] + "products";
            _blobServiceClient = blobServiceClient; // Initialize it
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Add IFormFile to accept the uploaded file
        public async Task<IActionResult> Create([Bind("Name,Price,Description")] Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // --- HANDLE IMAGE UPLOAD ---
                if (imageFile != null && imageFile.Length > 0)
                {
                    // 1. Get a client for the blob container (it will be created if it doesn't exist)
                    var containerClient = _blobServiceClient.GetBlobContainerClient("product-images");
                    await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                    // 2. Create a unique name for the blob to avoid overwrites
                    var blobName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    var blobClient = containerClient.GetBlobClient(blobName);

                    // 3. Upload the file to Blob Storage
                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    // 4. Set the product's ImageUrl to the public URL of the blob
                    product.ImageUrl = blobClient.Uri.ToString();
                }
                // --- END OF IMAGE HANDLING ---

                var client = _httpClientFactory.CreateClient();

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(product),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(_apiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"The API returned an error: {response.StatusCode} - {errorContent}");
                }
            }
            return View(product);
        }

        // --- All other methods remain unchanged ---

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var products = await client.GetFromJsonAsync<IEnumerable<Product>>(_apiUrl, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(products ?? new List<Product>());
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var product = await client.GetFromJsonAsync<Product>($"{_apiUrl}/{id}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Price,Description,ImageUrl,PartitionKey,RowKey,Timestamp,ETag")] Product product)
        {
            if (id != product.RowKey) return NotFound();
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                await client.PutAsJsonAsync($"{_apiUrl}/{id}", product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var product = await client.GetFromJsonAsync<Product>($"{_apiUrl}/{id}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}