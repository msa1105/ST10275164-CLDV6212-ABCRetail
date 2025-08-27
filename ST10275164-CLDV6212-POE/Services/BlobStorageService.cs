// In ST10275164-CLDV6212-POE/Services/BlobStorageService.cs

using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace ST10275164_CLDV6212_POE.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;
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
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                var blobClient = containerClient.GetBlobClient(fileName);
                fileStream.Position = 0;

                await blobClient.UploadAsync(fileStream, overwrite: true);

                _logger.LogInformation($"Successfully uploaded blob: {fileName}");
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file to blob storage: {fileName}");
                throw;
            }
        }

        public async Task<string> GetBlobUrlAsync(string containerName, string blobName) // Updated method
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting blob URL for: {blobName}");
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
                _ => "application/octet-stream"
            };
        }
    }
}