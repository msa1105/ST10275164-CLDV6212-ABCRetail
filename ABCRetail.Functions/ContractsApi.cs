using ABCRetail.Functions.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HttpMultipartParser;
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

            try
            {
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

                // Create container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var contracts = new List<ContractViewModel>();
                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    contracts.Add(new ContractViewModel
                    {
                        FileName = blobItem.Name,
                        FileUrl = blobClient.Uri.ToString()
                    });
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(contracts);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contracts");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Error retrieving contracts");
                return errorResponse;
            }
        }

        [Function("UploadContract")]
        public async Task<HttpResponseData> UploadContract(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "contracts")] HttpRequestData req)
        {
            _logger.LogInformation("Request to upload a new contract.");

            try
            {
                var parsedForm = await MultipartFormDataParser.ParseAsync(req.Body);
                var file = parsedForm.Files.FirstOrDefault();

                if (file == null)
                {
                    _logger.LogWarning("No file received in the upload request.");
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("No file uploaded");
                    return badResponse;
                }

                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(file.FileName);
                await blobClient.UploadAsync(file.Data, new BlobHttpHeaders { ContentType = file.ContentType });

                _logger.LogInformation($"Successfully uploaded {file.FileName}.");
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading contract.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error uploading contract: {ex.Message}");
                return errorResponse;
            }
        }
    }
}