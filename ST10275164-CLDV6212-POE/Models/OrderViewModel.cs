namespace ST10275164_CLDV6212_POE.Models
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }

        // Denormalised data to make it easier to display in the page
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
    }
}