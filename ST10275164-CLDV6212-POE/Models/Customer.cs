// Customer.cs
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ST10275164_CLDV6212_POE.Models
{
    public class Customer : ITableEntity
    {
        // Property for business logic (e.g., display)
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        // Properties required by ITableEntity
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}