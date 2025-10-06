using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
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

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var customers = await client.GetFromJsonAsync<IEnumerable<Customer>>(_apiUrl);
            return View(customers ?? new List<Customer>());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var customer = await client.GetFromJsonAsync<Customer>($"{_apiUrl}/{id}");
            if (customer == null) return NotFound();
            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Phone,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                await client.PostAsJsonAsync(_apiUrl, customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var customer = await client.GetFromJsonAsync<Customer>($"{_apiUrl}/{id}");
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Email,Phone,Address,PartitionKey,RowKey,Timestamp,ETag")] Customer customer)
        {
            if (id != customer.RowKey) return NotFound();
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                await client.PutAsJsonAsync($"{_apiUrl}/{id}", customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var customer = await client.GetFromJsonAsync<Customer>($"{_apiUrl}/{id}");
            if (customer == null) return NotFound();
            return View(customer);
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