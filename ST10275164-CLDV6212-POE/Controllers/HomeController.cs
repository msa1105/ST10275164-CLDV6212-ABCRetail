using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services; // Add this using directive for the services
using System.Diagnostics;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITableStorageService _tableStorageService; // Add the table storage service

        // Inject the ITableStorageService into the controller
        public HomeController(ILogger<HomeController> logger, ITableStorageService tableStorageService)
        {
            _logger = logger;
            _tableStorageService = tableStorageService;
        }

        // THE FIX: The Index action now fetches products and passes them to the view.
        public async Task<IActionResult> Index()
        {
            // Fetch all products, order them by timestamp, and take the most recent 3.
            var recentProducts = (await _tableStorageService.GetAllEntitiesAsync<Product>())
                                 .OrderByDescending(p => p.Timestamp)
                                 .Take(4)
                                 .ToList();

            // Pass the list of recent products to the view.
            return View(recentProducts);
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