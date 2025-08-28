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

        public async Task<IEnumerable<string>> GetAllFilesAsync()                   //list all files in the directory
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);

            var fileNames = new List<string>();

            await foreach (ShareFileItem file in directoryClient.GetFilesAndDirectoriesAsync())
            {
                fileNames.Add(file.Name);                                           // Add each file name to the list
            }

            return fileNames;
        }

        public async Task UploadFileAsync(string fileName, Stream fileStream)
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);        // Get a reference to the file share
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);    // Get a reference to the directory
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(fileName);               // Get a reference to the file

            fileStream.Position = 0;                                                // Reset stream position always to 0
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
        }
    }
}