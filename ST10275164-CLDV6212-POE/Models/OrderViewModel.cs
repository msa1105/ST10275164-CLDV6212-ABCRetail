namespace ST10275164_CLDV6212_POE.Models
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }

        // Denormalized data for easy display
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
    }
}