namespace ST10275164_CLDV6212_POE.Models
{
    public class QueueDetailsViewModel
    {
        public string QueueName { get; set; }
        public IEnumerable<string> Messages { get; set; } = new List<string>();
    }
}