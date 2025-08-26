namespace ST10275164_CLDV6212_POE.Services
{
    public interface IFileStorageService
    {
        Task UploadFileAsync(string fileName, Stream fileStream);
    }
}