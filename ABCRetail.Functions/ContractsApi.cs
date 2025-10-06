using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using HttpMultipartParser;
using System.Collections.Generic;
using ABCRetail.Functions.Models; // Add this using statement

namespace ABCRetail.Functions
{
    public class ContractsApi
    {
        private readonly ILogger<ContractsApi> _logger;
        private readonly ShareClient _shareClient;
        private readonly string _directoryName = "signed-agreements";

        public ContractsApi(ILogger<ContractsApi> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            _shareClient = new ShareClient(connectionString, "contracts");
            _shareClient.CreateIfNotExists();
        }

        // --- NEW FUNCTION TO GET THE LIST OF FILES ---
        [Function("GetContracts")]
        public async Task<HttpResponseData> GetContracts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "contracts")] HttpRequestData req)
        {
            _logger.LogInformation("Request to get all contracts.");
            var contractList = new List<ContractViewModel>();

            try
            {
                var directoryClient = _shareClient.GetDirectoryClient(_directoryName);

                await foreach (ShareFileItem file in directoryClient.GetFilesAndDirectoriesAsync())
                {
                    if (!file.IsDirectory)
                    {
                        var fileClient = directoryClient.GetFileClient(file.Name);
                        contractList.Add(new ContractViewModel
                        {
                            FileName = file.Name,
                            FileUrl = fileClient.Uri.ToString()
                        });
                    }
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(contractList);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching contracts.");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("UploadContract")]
        public async Task<HttpResponseData> UploadContract(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "contracts")] HttpRequestData req)
        {
            _logger.LogInformation("Request to upload a contract.");

            try
            {
                var parsedForm = await MultipartFormDataParser.ParseAsync(req.Body);
                var file = parsedForm.Files[0];

                if (file == null)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }

                var directoryClient = _shareClient.GetDirectoryClient(_directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(file.FileName);

                using (var stream = file.Data)
                {
                    await fileClient.CreateAsync(stream.Length);
                    await fileClient.UploadAsync(stream);
                }

                _logger.LogInformation($"File {file.FileName} uploaded successfully to {_directoryName}.");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"File {file.FileName} uploaded successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during file upload.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred on the server.");
                return errorResponse;
            }
        }
    }
}