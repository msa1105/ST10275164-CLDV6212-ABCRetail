// In ST10275164-CLDV6212-POE/Services/IBlobStorageService.cs

namespace ST10275164_CLDV6212_POE.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream);
        Task<string> GetBlobUrlAsync(string containerName, string blobName); // Renamed for clarity
        Task<bool> BlobExistsAsync(string fileName);
        Task DeleteBlobAsync(string fileName);
    }
}