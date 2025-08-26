using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace ST10275164_CLDV6212_POE.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;
        // The container where product images will be stored
        private const string ContainerName = "product-images";

        public BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream)
        {
            try
            {
                _logger.LogInformation($"Uploading file to blob storage: {fileName}");

                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

                // Create the container with no public access
                _logger.LogInformation($"Creating container if not exists: {ContainerName}");
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                var blobClient = containerClient.GetBlobClient(fileName);

                // Determine content type based on file extension
                var contentType = GetContentType(fileName);

                // Reset stream position to beginning
                fileStream.Position = 0;

                await blobClient.UploadAsync(fileStream, overwrite: true);

                _logger.LogInformation($"Successfully uploaded blob: {fileName}");

                // Return the blob URI - we'll handle access through different means
                // In production, you might want to implement a controller action to serve these files
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file to blob storage: {fileName}");
                throw;
            }
        }

        public async Task<string> GetBlobSasUrlAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                // For now, just return the blob URI
                // You can implement SAS token generation later if needed
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting blob URL for: {fileName}");
                throw;
            }
        }

        public async Task<bool> BlobExistsAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                var response = await blobClient.ExistsAsync();
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if blob exists: {fileName}");
                return false;
            }
        }

        public async Task DeleteBlobAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
                _logger.LogInformation($"Deleted blob: {fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting blob: {fileName}");
                throw;
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}