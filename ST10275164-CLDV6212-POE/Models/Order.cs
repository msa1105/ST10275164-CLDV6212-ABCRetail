using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ST10275164_CLDV6212_POE.Models
{
    public class Order : ITableEntity
    {
        public string OrderId { get; set; } = string.Empty;

        // These are the foreign keys linking to Customer and Product
        [Required]
        [Display(Name = "Customer")]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Product")]
        public string ProductId { get; set; } = string.Empty; // Im assuming one product per order for simplicity, can be expanded upon at a later date.

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }

        // These are the roperties that are required by ITableEntity
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }

    }
}