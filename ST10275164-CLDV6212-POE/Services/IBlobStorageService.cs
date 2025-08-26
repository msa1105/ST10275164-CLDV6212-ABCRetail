namespace ST10275164_CLDV6212_POE.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream);
        Task<string> GetBlobSasUrlAsync(string fileName);
        Task<bool> BlobExistsAsync(string fileName);
        Task DeleteBlobAsync(string fileName);
    }
}