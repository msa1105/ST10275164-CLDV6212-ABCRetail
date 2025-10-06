using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["FunctionApiUrl"] + "products";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Quantity,Category,Availability")] Product product)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();

                // --- THIS IS THE CRITICAL FIX ---
                // Manually serialize and create the content to be explicit
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(product),
                    Encoding.UTF8,
                    "application/json");

                // Use the more fundamental PostAsync method
                var response = await client.PostAsync(_apiUrl, jsonContent);
                // --- END OF FIX ---

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If the API returns an error, display it to the user
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
        public async Task<IActionResult> Edit(string id, [Bind("Name,Price,Quantity,Category,Availability,PartitionKey,RowKey,Timestamp,ETag")] Product product)
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