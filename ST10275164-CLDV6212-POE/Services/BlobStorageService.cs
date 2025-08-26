using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;

namespace ST10275164_CLDV6212_POE.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        // The container where product images will be stored
        private const string ContainerName = "product-images";

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            // Create the container if it doesn't exist.
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "image/jpeg" }); // Adjust content type as needed

            // Return the public URL of the uploaded blob
            return blobClient.Uri.ToString();
        }
    }
}