using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _ordersApiUrl;
        private readonly string _customersApiUrl;
        private readonly string _productsApiUrl;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<OrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _ordersApiUrl = configuration["FunctionApiUrl"] + "orders";
            _customersApiUrl = configuration["FunctionApiUrl"] + "customers";
            _productsApiUrl = configuration["FunctionApiUrl"] + "products";
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var orders = await client.GetFromJsonAsync<IEnumerable<OrderViewModel>>(_ordersApiUrl, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(orders ?? new List<OrderViewModel>());
        }

        // --- CHANGES START HERE ---

        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient();
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var customers = await client.GetFromJsonAsync<IEnumerable<Customer>>(_customersApiUrl, jsonOptions);
            var products = await client.GetFromJsonAsync<IEnumerable<Product>>(_productsApiUrl, jsonOptions);

            // Use the new Primary Key 'CustomerId' as the value
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name");

            // Use the new Primary Key 'ProductId' as the value
            ViewBag.ProductId = new SelectList(products, "ProductId", "Name");

            return View();
        }

        // --- END OF CHANGES ---

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,ProductId,TotalAmount")] Order order)
        {
            // This 'order' object from the form will now have int IDs
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(order),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(_ordersApiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API Error: {response.StatusCode} - {errorContent}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    _logger.LogError($"Validation Error: {error.ErrorMessage}");
                }
            }

            // --- CHANGES FOR RE-POPULATING DROPDOWN ON FAILURE ---
            var customers = await _httpClientFactory.CreateClient().GetFromJsonAsync<IEnumerable<Customer>>(_customersApiUrl);
            var products = await _httpClientFactory.CreateClient().GetFromJsonAsync<IEnumerable<Product>>(_productsApiUrl);

            // Use 'CustomerId' and 'ProductId'
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name", order.CustomerId);
            ViewBag.ProductId = new SelectList(products, "ProductId", "Name", order.ProductId);

            return View(order);
        }
    }
}