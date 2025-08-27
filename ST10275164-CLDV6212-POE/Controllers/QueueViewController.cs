using Microsoft.AspNetCore.Mvc;
using ST10275164_CLDV6212_POE.Models; // Add this using directive for the models
using ST10275164_CLDV6212_POE.Services;

namespace ST10275164_CLDV6212_POE.Controllers
{
    public class QueueViewController : Controller
    {
        private readonly IQueueStorageService _queueStorageService;

        public QueueViewController(IQueueStorageService queueStorageService)
        {
            _queueStorageService = queueStorageService;
        }

        public async Task<IActionResult> Index()
        {
            // Use the new QueueViewModel to pass data to the view
            var viewModel = new QueueViewModel
            {
                QueueNames = await _queueStorageService.GetQueuesAsync() ?? new List<string>()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                return RedirectToAction(nameof(Index));
            }

            // Use the new QueueDetailsViewModel to pass data to the view
            var viewModel = new QueueDetailsViewModel
            {
                QueueName = queueName,
                Messages = await _queueStorageService.GetMessagesAsync(queueName) ?? new List<string>()
            };

            return View(viewModel);
        }
    }
}