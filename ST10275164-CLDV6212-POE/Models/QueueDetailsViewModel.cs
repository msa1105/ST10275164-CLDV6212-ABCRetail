namespace ST10275164_CLDV6212_POE.Models
{
    public class QueueDetailsViewModel
    {
        public string QueueName { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}