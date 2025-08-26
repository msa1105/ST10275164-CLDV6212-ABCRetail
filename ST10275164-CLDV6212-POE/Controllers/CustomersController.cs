using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ITableStorageService _tableStorageService;

        public CustomersController(ITableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        // GET: /Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            return View(customers);
        }

        // GET: /Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Customers/Create
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
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
    }
}