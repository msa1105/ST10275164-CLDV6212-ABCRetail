namespace ABCRetail.Functions.Models;

public class QueueDetailsViewModel
{
    public string QueueName { get; set; }
    public List<string> Messages { get; set; } = new List<string>();
}