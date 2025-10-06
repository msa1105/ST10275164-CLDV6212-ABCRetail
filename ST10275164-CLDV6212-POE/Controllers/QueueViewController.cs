using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models;
using System.Net.Http.Json;
using System.Text;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class QueueViewController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public QueueViewController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["FunctionApiUrl"] + "queues";
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var queueNames = await client.GetFromJsonAsync<List<string>>(_apiUrl);
            var viewModel = new QueueViewModel { QueueNames = queueNames ?? new List<string>() };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string queueName)
        {
            if (!string.IsNullOrEmpty(queueName))
            {
                var client = _httpClientFactory.CreateClient();
                await client.PostAsync($"{_apiUrl}/{queueName}", null);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string queueName) 
        {
            if (string.IsNullOrEmpty(queueName)) return NotFound();
            var client = _httpClientFactory.CreateClient();
            var messages = await client.GetFromJsonAsync<List<string>>($"{_apiUrl}/{queueName}/messages");
            var viewModel = new QueueDetailsViewModel
            {
                QueueName = queueName,
                Messages = messages ?? new List<string>()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMessage(string queueName, string messageContent)
        {
            if (!string.IsNullOrEmpty(queueName) && !string.IsNullOrEmpty(messageContent))
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(messageContent, Encoding.UTF8, "text/plain");
                await client.PostAsync($"{_apiUrl}/{queueName}/messages", content);
            }
            return RedirectToAction(nameof(Details), new { queueName = queueName }); 
        }
    }
}