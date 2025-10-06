using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["FunctionApiUrl"] + "products"; //makes sure to point to products api
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            IEnumerable<Product> products = new List<Product>();
            try
            {
                products = await client.GetFromJsonAsync<IEnumerable<Product>>(_apiUrl, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch products for home page.");
                // Return the view with an empty list if the API call fails
            }

            return View(products ?? new List<Product>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}