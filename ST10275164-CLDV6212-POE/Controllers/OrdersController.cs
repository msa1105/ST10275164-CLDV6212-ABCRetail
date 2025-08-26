using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IQueueStorageService _queueStorageService;

        public OrdersController(ITableStorageService tableStorageService, IQueueStorageService queueStorageService)
        {
            _tableStorageService = tableStorageService;
            _queueStorageService = queueStorageService;
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            // Load customers and products to populate dropdown lists
            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name");
            ViewBag.ProductId = new SelectList(products, "ProductId", "Name");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Set order details
                order.OrderId = Guid.NewGuid().ToString();
                order.PartitionKey = "Order";
                order.RowKey = order.OrderId;
                order.OrderDate = DateTime.UtcNow;

                // Save order to Table Storage
                await _tableStorageService.UpsertEntityAsync(order);

                // Send a message to Queue Storage to notify of the new order
                await _queueStorageService.SendMessageAsync($"New order created: {order.OrderId}");

                // Redirect to a confirmation page or home page
                return RedirectToAction("Index", "Home");
            }

            // If model is invalid, reload dropdowns
            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name", order.CustomerId);
            ViewBag.ProductId = new SelectList(products, "ProductId", "Name", order.ProductId);
            return View(order);
        }
    }
}
