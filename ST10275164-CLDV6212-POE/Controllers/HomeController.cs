using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services; 
using System.Diagnostics;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITableStorageService _tableStorageService; 

        
        public HomeController(ILogger<HomeController> logger, ITableStorageService tableStorageService)
        {
            _logger = logger;
            _tableStorageService = tableStorageService;
        }

        
        public async Task<IActionResult> Index()
        {
            // Fetches all products, order them by timestamp, and take the most recent 3.
            var recentProducts = (await _tableStorageService.GetAllEntitiesAsync<Product>())
                                 .OrderByDescending(p => p.Timestamp)
                                 .Take(3)
                                 .ToList();

            // Passes the list of recent products to the view.
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