namespace ST10275164_CLDV6212_POE.Services
{
    public interface IFileStorageService
    {
        Task UploadFileAsync(string fileName, Stream fileStream);   // Method to upload a file


        Task<IEnumerable<string>> GetAllFilesAsync();               // Method to list all files in the directory
    }
}

//boring interface