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

        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllEntitiesAsync<Order>();
            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();

            // using ToDictionary for efficient lookups when mapping names
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

    
        public async Task<IActionResult> Create()
        {
            var customers = await _tableStorageService.GetAllEntitiesAsync<Customer>();
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name");
            ViewBag.ProductId = new SelectList(products, "ProductId", "Name");
            return View();
        }

       
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