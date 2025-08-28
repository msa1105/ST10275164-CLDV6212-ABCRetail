using Azure.Storage.Blobs.Models; // (Microsoft, 2024a)
using Azure.Storage.Blobs;        // (Microsoft, 2024a)

namespace ST10275164_CLDV6212_POE.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient; // (Microsoft, 2024a)
        private readonly ILogger<BlobStorageService> _logger;  // (Microsoft, 2024b: Logging)
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
                _logger.LogInformation($"Uploading file to blob storage: {fileName}"); // (Microsoft, 2024b)

                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName); // (Microsoft, 2024a)
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);           // (Microsoft, 2024a)

                var blobClient = containerClient.GetBlobClient(fileName);                      // (Microsoft, 2024a)
                fileStream.Position = 0;

                await blobClient.UploadAsync(fileStream, overwrite: true);                     // (Microsoft, 2024a)

                _logger.LogInformation($"Successfully uploaded blob: {fileName}");             // (Microsoft, 2024b)
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file to blob storage: {fileName}");     // (Microsoft, 2024b)
                throw;
            }
        }

        public async Task<string> GetBlobUrlAsync(string containerName, string blobName) // Updated method
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName); // (Microsoft, 2024a)
                var blobClient = containerClient.GetBlobClient(blobName);                       // (Microsoft, 2024a)
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting blob URL for: {blobName}");                // (Microsoft, 2024b)
                throw;
            }
        }

        public async Task<bool> BlobExistsAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName); // (Microsoft, 2024a)
                var blobClient = containerClient.GetBlobClient(fileName);                       // (Microsoft, 2024a)
                var response = await blobClient.ExistsAsync();                                  // (Microsoft, 2024a)
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if blob exists: {fileName}");              // (Microsoft, 2024b)
                return false;
            }
        }

        public async Task DeleteBlobAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName); // (Microsoft, 2024a)
                var blobClient = containerClient.GetBlobClient(fileName);                       // (Microsoft, 2024a)
                await blobClient.DeleteIfExistsAsync();                                         // (Microsoft, 2024a)
                _logger.LogInformation($"Deleted blob: {fileName}");                            // (Microsoft, 2024b)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting blob: {fileName}");                       // (Microsoft, 2024b)
                throw;
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant(); // (Microsoft, 2024c: Path)
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


//Microsoft, 2024a. Upload and download blobs with .NET. [online] Available at: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload
//[Accessed 24 August 2025].

//Microsoft, 2024b.Logging in .NET. [online] Available at: https://learn.microsoft.com/en-us/dotnet/core/extensions/logging
//[Accessed 24 August 2025].

//Microsoft, 2024c.Path Class(System.IO). [online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.io.path
//[Accessed 24 August 2025].


//I wasn't too sure how to build these best practice so I did alot of researching and had help from ChatGPT as well.