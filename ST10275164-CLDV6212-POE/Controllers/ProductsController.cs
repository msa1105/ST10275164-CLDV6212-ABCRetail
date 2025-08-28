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

        public async Task<IActionResult> Index() // (Microsoft, 2023: Asynchronous Programming)
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

//Microsoft (2023) Asynchronous programming with async and await. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ (Accessed: 25 August 2025).

//Microsoft(2024a) Overview of ASP.NET Core MVC. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-8.0 (Accessed: 25 August 2025).

//Microsoft(2024b) Dependency injection in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0 (Accessed: 28 August 2025).

//Microsoft(2024c) Language Integrated Query (LINQ). Available at: https://learn.microsoft.com/en-us/dotnet/csharp/linq/ (Accessed: 25 August 2025).

//Microsoft(2024d) Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-8.0 (Accessed: 28 August 2025).

//Microsoft(2024e) File uploads in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0 (Accessed: 25 August 2025).