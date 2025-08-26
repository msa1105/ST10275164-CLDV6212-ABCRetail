using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Services;
using ST10275164_CLDV6212_POE.Models;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;

        public ProductsController(ITableStorageService tableStorageService, IBlobStorageService blobStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
        }

        // GET: /Products
        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllEntitiesAsync<Product>();
            return View(products);
        }

        // GET: /Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Generate a unique file name to prevent overwrites
                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";

                    // Upload the file to Azure Blob Storage
                    using (var stream = imageFile.OpenReadStream())
                    {
                        var imageUrl = await _blobStorageService.UploadFileToBlobAsync(uniqueFileName, stream);
                        product.ImageUrl = imageUrl; // Save the public URL to the product model
                    }
                }

                // Set PartitionKey and RowKey for the new product
                product.ProductId = Guid.NewGuid().ToString();
                product.PartitionKey = "Product";
                product.RowKey = product.ProductId;

                // Save the product metadata to Azure Table Storage
                await _tableStorageService.UpsertEntityAsync(product);

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}