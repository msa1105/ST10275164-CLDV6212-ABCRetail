using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

//Codeproject.com. (2025). CodeProject. [online]
//Available at:
//https://www.codeproject.com/Articles/490178/How-to-Use-Azure-Blob-Storage-with-Azure-Web-Sites [Accessed 24 Aug. 2025].

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobController> _logger;

        public BlobController(BlobServiceClient blobServiceClient, ILogger<BlobController> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        
        [HttpGet("Blob/Image/{fileName}")]  //Stack Overflow. (n.d.). HttpPost vs HttpGet attributes in MVC: Why use HttpPost?
                                            //[online] Available at:
                                            //https://stackoverflow.com/questions/5332275/httppost-vs-httpget-attributes-in-mvc-why-use-httppost.‌
        public async Task<IActionResult> Image(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return NotFound();
                }

                var containerClient = _blobServiceClient.GetBlobContainerClient("product-images");
                var blobClient = containerClient.GetBlobClient(fileName);

                // Checks if blob exists
                var exists = await blobClient.ExistsAsync();
                if (!exists.Value)
                {
                    _logger.LogWarning($"Blob not found: {fileName}");
                    return NotFound();
                }

                // Downloads the blob
                var response = await blobClient.DownloadAsync();
                var contentType = response.Value.Details.ContentType ?? "application/octet-stream";

                _logger.LogInformation($"Serving blob: {fileName} with content type: {contentType}");

                return File(response.Value.Content, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error serving blob image: {fileName}");
                return StatusCode(500, "Error retrieving image");
            }
        }
    }
}