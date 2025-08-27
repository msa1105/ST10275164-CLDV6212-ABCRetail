using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IQueueStorageService _queueStorageService; // --- FIX: Declare the service ---

        // --- FIX: Inject the service in the constructor ---
        public CustomersController(ITableStorageService tableStorageService, IQueueStorageService queueStorageService)
        {
            _tableStorageService = tableStorageService;
            _queueStorageService = queueStorageService; // --- FIX: Assign the service ---
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CustomerId = Guid.NewGuid().ToString();
                customer.PartitionKey = "Customer";
                customer.RowKey = customer.CustomerId;
                await _tableStorageService.UpsertEntityAsync(customer);

                // This line will now work correctly
                await _queueStorageService.SendMessageAsync("customer-events", $"New Customer Created: {customer.Name} (ID: {customer.CustomerId})");

                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
    }
}