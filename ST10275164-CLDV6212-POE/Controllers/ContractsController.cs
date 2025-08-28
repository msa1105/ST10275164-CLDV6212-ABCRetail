using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    // Controller class serves as the base for MVC controllers, handling incoming browser requests.
    public class ContractsController : Controller
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfiguration _configuration;
        private readonly IQueueStorageService _queueStorageService;

        // The constructor utilizes dependency injection to provide instances of required services.
        // This is a core pattern in ASP.NET Core for achieving loosely coupled code (Microsoft, 2024b).
        public ContractsController(IFileStorageService fileStorageService, IConfiguration configuration, IQueueStorageService queueStorageService)
        {
            _fileStorageService = fileStorageService;
            _configuration = configuration;
            _queueStorageService = queueStorageService;
        }


        // This action uses the async/await pattern to perform non-blocking I/O operations,
        // improving the application's responsiveness and scalability (Microsoft, 2023).
        public async Task<IActionResult> Index()
        {
            var fileNames = await _fileStorageService.GetAllFilesAsync();


            var storageAccountName = _configuration["StorageAccountName"];
            var baseUrl = $"https://{storageAccountName}.file.core.windows.net/contracts/signed-agreements/";

            // Language Integrated Query (LINQ) is used here to project the list of file names
            // into a new collection of ContractViewModel objects (Microsoft, 2024c).
            var contracts = fileNames.Select(name => new ContractViewModel
            {
                FileName = name,
                FileUrl = $"{baseUrl}{name}"
            }).ToList();

            return View(contracts);
        }

        [HttpPost] // Attribute to specify that this action handles HTTP POST requests.
        // This attribute adds a token to prevent Cross-Site Request Forgery (CSRF) attacks,
        // a common web security vulnerability (Microsoft, 2024d).
        [ValidateAntiForgeryToken]
        // The IFormFile parameter represents a file sent with the HTTP request,
        // which is the standard way to handle file uploads in ASP.NET Core (Microsoft, 2024e).
        public async Task<IActionResult> Upload(IFormFile contractFile)
        {
            if (contractFile != null && contractFile.Length > 0)
            {
                var fileName = Path.GetFileName(contractFile.FileName);
                using (var stream = contractFile.OpenReadStream())
                {
                    await _fileStorageService.UploadFileAsync(fileName, stream);
                    await _queueStorageService.SendMessageAsync("contract-events", $"New Contract Uploaded: {fileName}");
                }
                // TempData is used to store data that persists only for the subsequent request.
                TempData["Message"] = "Contract uploaded successfully!";
            }
            else
            {
                TempData["Message"] = "Please select a file to upload.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

//Microsoft (2023) Asynchronous programming with async and await. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ (Accessed: 28 August 2025).

//Microsoft(2024a) Overview of ASP.NET Core MVC. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-8.0 (Accessed: 28 August 2025).

//Microsoft(2024b) Dependency injection in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0 (Accessed: 28 August 2025).

//Microsoft(2024c) Language Integrated Query (LINQ). Available at: https://learn.microsoft.com/en-us/dotnet/csharp/linq/ (Accessed: 28 August 2025).

//Microsoft(2024d) Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-8.0 (Accessed: 28 August 2025).

//Microsoft(2024e) File uploads in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0 (Accessed: 28 August 2025).