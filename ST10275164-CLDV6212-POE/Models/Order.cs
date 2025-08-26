using Azure;
using Azure.Data.Tables;

namespace ST10275164_CLDV6212_POE.Models
{
    public class Order : ITableEntity
    {
        // Property for business logic
            public string OrderId { get; set; }

        // Foreign keys linking to Customer and Product
            public string CustomerId { get; set; }
            public string ProductId { get; set; } // Assuming one product per order for simplicity

            public DateTime OrderDate { get; set; }

            public double TotalAmount { get; set; }

        // Properties required by ITableEntity
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
    }
}