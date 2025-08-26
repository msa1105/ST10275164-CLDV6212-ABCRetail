using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

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

        // GET: /Blob/Image/filename.jpg
        [HttpGet("Blob/Image/{fileName}")]
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

                // Check if blob exists
                var exists = await blobClient.ExistsAsync();
                if (!exists.Value)
                {
                    _logger.LogWarning($"Blob not found: {fileName}");
                    return NotFound();
                }

                // Download the blob
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