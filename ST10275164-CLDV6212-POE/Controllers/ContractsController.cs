using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IFileStorageService _fileStorageService;

        public ContractsController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        // GET: /Contracts/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /Contracts/Upload
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