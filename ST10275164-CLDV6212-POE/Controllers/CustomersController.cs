// CustomersController.cs
using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ITableStorageService tableStorageService, ILogger<CustomersController> logger)
        {
            _tableStorageService = tableStorageService;
            _logger = logger;
        }

        // GET: /Customers
        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
                _logger.LogInformation($"Retrieved {customers.Count} customers from Azure Table Storage");
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers from Azure Table Storage");
                ViewBag.Error = "Error loading customers. Please try again.";
                return View(new List<Customer>());
            }
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
                try
                {
                    customer.CustomerId = Guid.NewGuid().ToString();
                    customer.PartitionKey = "Customer";
                    customer.RowKey = customer.CustomerId;

                    _logger.LogInformation($"Attempting to create customer: {customer.Name} with ID: {customer.CustomerId}");

                    await _tableStorageService.UpsertEntityAsync(customer);

                    _logger.LogInformation($"Successfully created customer: {customer.CustomerId}");
                    TempData["Success"] = "Customer created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating customer: {customer.Name}");
                    ViewBag.Error = "Error creating customer. Please try again.";
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid for customer creation");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                }
            }

            return View(customer);
        }
    }
}
