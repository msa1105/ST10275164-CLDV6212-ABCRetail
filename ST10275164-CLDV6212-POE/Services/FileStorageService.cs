using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ST10275164_CLDV6212_POE.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ShareServiceClient _shareServiceClient;
        private const string ShareName = "contracts";
        private const string DirectoryName = "signed-agreements";

        public FileStorageService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
        }

        // NEW METHOD IMPLEMENTATION
        public async Task<IEnumerable<string>> GetAllFilesAsync()
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);

            var fileNames = new List<string>();

            await foreach (ShareFileItem file in directoryClient.GetFilesAndDirectoriesAsync())
            {
                fileNames.Add(file.Name);
            }

            return fileNames;
        }

        public async Task UploadFileAsync(string fileName, Stream fileStream)
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(fileName);

            fileStream.Position = 0; // Ensure the stream is at the beginning
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
        }
    }
}