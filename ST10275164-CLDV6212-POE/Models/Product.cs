using Azure;
using Azure.Data.Tables;

namespace ST10275164_CLDV6212_POE.Models
{
    public class Product : ITableEntity
    {
        // Property for business logic
            public string ProductId { get; set; }

            public string Name { get; set; }

            public double Price { get; set; }

            public string Description { get; set; }

        // This will store the public URL of the image from Blob Storage
            public string ImageUrl { get; set; }

            // Properties required by ITableEntity
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
        }
}