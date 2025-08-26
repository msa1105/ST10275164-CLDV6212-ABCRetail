// Product.cs
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ST10275164_CLDV6212_POE.Models
{
    public class Product : ITableEntity
    {
        // Property for business logic
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Product Description")]
        public string Description { get; set; } = string.Empty;

        // This will store the public URL of the image from Blob Storage
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        // Properties required by ITableEntity
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}

