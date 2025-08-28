using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueStorageService _queueStorageService; 

        
        public ProductsController(ITableStorageService tableStorageService, IBlobStorageService blobStorageService, IQueueStorageService queueStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _queueStorageService = queueStorageService; 
        }

        public async Task<IActionResult> Index() // (Microsoft, 2024: Asynchronous Programming)
        {
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid) // (Microsoft, 2024: Model Validation)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    using (var stream = imageFile.OpenReadStream())
                    {
                        var imageUrl = await _blobStorageService.UploadFileToBlobAsync(uniqueFileName, stream);
                        product.ImageUrl = imageUrl;
                    }
                }

                product.ProductId = Guid.NewGuid().ToString();
                product.PartitionKey = "Product";
                product.RowKey = product.ProductId;

                await _tableStorageService.UpsertEntityAsync(product);

                
                await _queueStorageService.SendMessageAsync("product-events", $"New Product Created: {product.Name} (ID: {product.ProductId})");

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var product = await _tableStorageService.GetEntityAsync<Product>("Product", id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}