using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

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

        public async Task<IActionResult> Index()
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
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    using (var stream = imageFile.OpenReadStream())
                    {
                        // 1. The service uploads the image and returns its public URL.
                        var imageUrl = await _blobStorageService.UploadFileToBlobAsync(uniqueFileName, stream);

                        // 2. (THE FIX) We assign the returned URL to the product's ImageUrl property.
                        product.ImageUrl = imageUrl;
                    }
                }

                product.ProductId = Guid.NewGuid().ToString();
                product.PartitionKey = "Product";
                product.RowKey = product.ProductId;

                // 3. We now save the complete product object, including the ImageUrl, to the database.
                await _tableStorageService.UpsertEntityAsync(product);

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // This Details action is for the clickable cards feature. No changes needed here.
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _tableStorageService.GetEntityAsync<Product>("Product", id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}