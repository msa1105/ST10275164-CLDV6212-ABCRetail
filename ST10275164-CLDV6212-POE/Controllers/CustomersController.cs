using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;

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
    }
}