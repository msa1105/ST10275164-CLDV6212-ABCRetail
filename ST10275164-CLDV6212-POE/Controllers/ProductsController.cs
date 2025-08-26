using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Services;
using ST10275164_CLDV6212_POE.Models;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ITableStorageService tableStorageService, IBlobStorageService blobStorageService, ILogger<ProductsController> logger)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        // GET: /Products
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _tableStorageService.GetAllEntitiesAsync<Product>();
                _logger.LogInformation($"Retrieved {products.Count} products from Azure Table Storage");
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products from Azure Table Storage");
                ViewBag.Error = "Error loading products. Please try again.";
                return View(new List<Product>());
            }
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
            _logger.LogInformation($"Product creation attempt started for: {product?.Name ?? "null"}");
            _logger.LogInformation($"ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for product creation");
                foreach (var error in ModelState)
                {
                    _logger.LogWarning($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                ViewBag.Error = "Please correct the validation errors and try again.";
                return View(product);
            }

            try
            {
                // Log incoming data
                _logger.LogInformation($"Creating product: Name='{product.Name}', Price={product.Price}, Description='{product.Description}'");
                _logger.LogInformation($"Image file: {(imageFile != null ? $"Name={imageFile.FileName}, Size={imageFile.Length}" : "None")}");

                product.ProductId = Guid.NewGuid().ToString();
                product.PartitionKey = "Product";
                product.RowKey = product.ProductId;

                _logger.LogInformation($"Assigned ProductId: {product.ProductId}");

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Validate image file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        _logger.LogWarning($"Invalid file extension: {fileExtension}");
                        ViewBag.Error = "Only image files are allowed (jpg, jpeg, png, gif, bmp).";
                        return View(product);
                    }

                    if (imageFile.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        _logger.LogWarning($"File too large: {imageFile.Length} bytes");
                        ViewBag.Error = "Image file must be smaller than 5MB.";
                        return View(product);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    _logger.LogInformation($"Uploading image file: {uniqueFileName}");

                    try
                    {
                        using (var stream = imageFile.OpenReadStream())
                        {
                            await _blobStorageService.UploadFileToBlobAsync(uniqueFileName, stream);
                            product.ImageUrl = uniqueFileName;
                            _logger.LogInformation($"Image uploaded successfully: {uniqueFileName}");
                        }
                    }
                    catch (Exception blobEx)
                    {
                        _logger.LogError(blobEx, $"Error uploading image: {uniqueFileName}");
                        ViewBag.Error = "Error uploading image. Please try again.";
                        return View(product);
                    }
                }
                else
                {
                    _logger.LogInformation("No image file provided");
                }

                // Save the product metadata to Azure Table Storage
                _logger.LogInformation($"Saving product to table storage: {product.ProductId}");
                await _tableStorageService.UpsertEntityAsync(product);

                _logger.LogInformation($"Successfully created product: {product.ProductId}");
                TempData["Success"] = $"Product '{product.Name}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating product: {product?.Name ?? "unknown"}");
                ViewBag.Error = $"Error creating product: {ex.Message}";
                return View(product);
            }
        }
    }
}