namespace ST10275164_CLDV6212_POE.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream); // upload file to blob storage
        Task<string> GetBlobUrlAsync(string containerName, string blobName);    // get the URL of a blob
        Task<bool> BlobExistsAsync(string fileName);                            // check if a blob exists
        Task DeleteBlobAsync(string fileName);                                  // delete a blob (once again will be used in admin moves)
    }
}

//boring interface