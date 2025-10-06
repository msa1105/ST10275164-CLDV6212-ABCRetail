using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ST10275164_CLDV6212_POE.Models; // Add this using statement
using System.Net.Http.Json; // Add this using statement

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public ContractsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["FunctionApiUrl"] + "contracts";
        }

        // --- UPDATED INDEX ACTION ---
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var contracts = await client.GetFromJsonAsync<IEnumerable<ContractViewModel>>(_apiUrl);
            return View(contracts ?? new List<ContractViewModel>());
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile contractFile)
        {
            if (contractFile != null && contractFile.Length > 0)
            {
                var client = _httpClientFactory.CreateClient();

                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new StreamContent(contractFile.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "contractFile",
                        FileName = contractFile.FileName
                    };
                    content.Add(fileContent);

                    var response = await client.PostAsync(_apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "File uploaded successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"API Error: {response.StatusCode} - {errorContent}");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("contractFile", "Please select a file to upload.");
            }

            return View();
        }
    }
}