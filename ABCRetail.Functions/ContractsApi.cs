using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HttpMultipartParser; // ADD THIS LINE
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ABCRetail.Functions
{
    public class ContractsApi
    {
        private readonly ILogger<ContractsApi> _logger;
        private const string ContainerName = "contracts";

        public ContractsApi(ILogger<ContractsApi> logger)
        {
            _logger = logger;
        }

        [Function("GetContracts")]
        public async Task<HttpResponseData> GetContracts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "contracts")] HttpRequestData req)
        {
            _logger.LogInformation("Request to list all contracts.");
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

            var blobUris = new List<string>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobUris.Add(containerClient.GetBlobClient(blobItem.Name).Uri.ToString());
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(blobUris);
            return response;
        }

        [Function("UploadContract")]
        public async Task<HttpResponseData> UploadContract(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "contracts")] HttpRequestData req)
        {
            _logger.LogInformation("Request to upload a new contract.");

            try
            {
                // Use the new library to parse the request body
                var parsedForm = await MultipartFormDataParser.ParseAsync(req.Body);
                var file = parsedForm.Files.FirstOrDefault(); // Get the first file

                if (file == null)
                {
                    _logger.LogWarning("No file received in the upload request.");
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }

                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(file.FileName);

                // Use the file's data stream to upload
                await blobClient.UploadAsync(file.Data, new BlobHttpHeaders { ContentType = file.ContentType });

                _logger.LogInformation($"Successfully uploaded {file.FileName}.");
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading contract.");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}