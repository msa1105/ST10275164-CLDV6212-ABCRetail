using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _functionApiUrl;

        public OrdersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _functionApiUrl = configuration["FunctionApiUrl"];
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var orders = await client.GetFromJsonAsync<IEnumerable<Order>>($"{_functionApiUrl}orders");
            return View(orders ?? new List<Order>());
        }

        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient();

            // Make two API calls to get the data for the dropdowns
            var customers = await client.GetFromJsonAsync<IEnumerable<Customer>>($"{_functionApiUrl}customers");
            var products = await client.GetFromJsonAsync<IEnumerable<Product>>($"{_functionApiUrl}products");

            var viewModel = new CreateOrderViewModel
            {
                Customers = new SelectList(customers, "RowKey", "Name"),
                Products = new SelectList(products, "RowKey", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                await client.PostAsJsonAsync($"{_functionApiUrl}orders", viewModel.Order);
                return RedirectToAction(nameof(Index));
            }

            // If the submission fails, we need to repopulate the dropdowns
            var clientForDropdowns = _httpClientFactory.CreateClient();
            var customers = await clientForDropdowns.GetFromJsonAsync<IEnumerable<Customer>>($"{_functionApiUrl}customers");
            var products = await clientForDropdowns.GetFromJsonAsync<IEnumerable<Product>>($"{_functionApiUrl}products");
            viewModel.Customers = new SelectList(customers, "RowKey", "Name", viewModel.Order.CustomerId);
            viewModel.Products = new SelectList(products, "RowKey", "Name", viewModel.Order.ProductId);

            return View(viewModel);
        }
    }
}