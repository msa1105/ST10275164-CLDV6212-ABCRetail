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

        // NEW ACTION TO DISPLAY ALL ORDERS
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllEntitiesAsync<Order>();
            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();

            // Use ToDictionary for efficient lookups
            var customerDict = customers.ToDictionary(c => c.RowKey, c => c.Name);
            var productDict = products.ToDictionary(p => p.RowKey, p => p.Name);

            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                CustomerName = customerDict.ContainsKey(o.CustomerId) ? customerDict[o.CustomerId] : "N/A",
                ProductName = productDict.ContainsKey(o.ProductId) ? productDict[o.ProductId] : "N/A"
            }).ToList();

            return View(orderViewModels);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
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
                order.OrderId = Guid.NewGuid().ToString();
                order.PartitionKey = "Order";
                order.RowKey = order.OrderId;
                order.OrderDate = DateTime.UtcNow;

                // Update the following line in the POST: Orders/Create method
                await _queueStorageService.SendMessageAsync("orders-queue", $"New order created: {order.OrderId}");
                await _tableStorageService.UpsertEntityAsync(order);
                

                return RedirectToAction(nameof(Index));
            }

            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name", order.CustomerId);
            ViewBag.ProductId = new SelectList(products, "ProductId", "Name", order.ProductId);
            return View(order);
        }
    }
}