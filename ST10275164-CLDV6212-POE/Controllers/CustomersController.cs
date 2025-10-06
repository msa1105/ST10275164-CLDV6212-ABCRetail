using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public CustomersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["FunctionApiUrl"] + "customers";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();

                // --- APPLY THE SAME FIX HERE ---
                // Manually serialize and create the content to be explicit
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(customer),
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
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"The API returned an error: {response.StatusCode} - {errorContent}");
                }
            }
            return View(customer);
        }

        // --- All other methods remain unchanged ---

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var customers = await client.GetFromJsonAsync<IEnumerable<Customer>>(_apiUrl, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(customers ?? new List<Customer>());
        }

        public IActionResult Create()
        {
            return View();
        }

        // ... (Keep the rest of the controller methods as they are)
    }
}