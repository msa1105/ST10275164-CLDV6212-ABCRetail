using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;
        private readonly BlobServiceClient _blobServiceClient;

        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["FunctionApiUrl"] + "products";
            _blobServiceClient = blobServiceClient;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Description")] Product product, IFormFile imageFile)
        {
            // --- THIS IMAGE UPLOAD LOGIC IS UNAFFECTED ---
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient("product-images");
                    await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                    var blobName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    var blobClient = containerClient.GetBlobClient(blobName);
                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                    product.ImageUrl = blobClient.Uri.ToString();
                }

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

        // --- CHANGES START HERE ---

        public async Task<IActionResult> Edit(int id) // <-- Changed to int
        {
            if (id == 0) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var product = await client.GetFromJsonAsync<Product>($"{_apiUrl}/{id}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Changed id to int, updated Bind properties
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Price,Description,ImageUrl")] Product product)
        {
            if (id != product.ProductId) return NotFound(); // <-- Compare with ProductId

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                await client.PutAsJsonAsync($"{_apiUrl}/{id}", product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int id) // <-- Changed to int
        {
            if (id == 0) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var product = await client.GetFromJsonAsync<Product>($"{_apiUrl}/{id}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // <-- Changed to int
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}