using Azure.Storage.Files.Shares;

namespace ST10275164_CLDV6212_POE.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ShareServiceClient _shareServiceClient;
        // Name of the file share where contracts will be stored
        private const string ShareName = "contracts";
        // Name of the directory within the share
        private const string DirectoryName = "signed-agreements";

        public FileStorageService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
        }

        public async Task UploadFileAsync(string fileName, Stream fileStream)
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(fileName);

            // Upload the file stream
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
        }
    }
}