using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ST10275164_CLDV6212_POE.Models
{
    public class Customer : ITableEntity
    {
        public string CustomerId { get; set; } = string.Empty;

        [Required] // I made name and email required because when adding a customer the table needs to be populated with that data
        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Required]  //Ref: UP
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}