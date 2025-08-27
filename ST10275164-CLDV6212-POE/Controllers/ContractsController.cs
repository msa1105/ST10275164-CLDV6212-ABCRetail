using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfiguration _configuration;

        public ContractsController(IFileStorageService fileStorageService, IConfiguration configuration)
        {
            _fileStorageService = fileStorageService;
            _configuration = configuration;
        }

        // NEW INDEX ACTION
        public async Task<IActionResult> Index()
        {
            var fileNames = await _fileStorageService.GetAllFilesAsync();

            // Construct the base URL for the file share
            var storageAccountName = _configuration["StorageAccountName"]; // You'll add this to appsettings
            var baseUrl = $"https://{storageAccountName}.file.core.windows.net/contracts/signed-agreements/";

            var contracts = fileNames.Select(name => new ContractViewModel
            {
                FileName = name,
                FileUrl = $"{baseUrl}{name}" // We need to add a SAS token for this to be viewable
            }).ToList();

            return View(contracts);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile contractFile)
        {
            if (contractFile != null && contractFile.Length > 0)
            {
                var fileName = Path.GetFileName(contractFile.FileName);
                using (var stream = contractFile.OpenReadStream())
                {
                    await _fileStorageService.UploadFileAsync(fileName, stream);
                }
                ViewBag.Message = "File uploaded successfully!";
            }
            else
            {
                ViewBag.Message = "Please select a file to upload.";
            }

            return View();
        }
    }
}