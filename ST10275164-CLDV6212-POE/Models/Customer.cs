using Azure;
using Azure.Data.Tables;

namespace ST10275164_CLDV6212_POE.Models
{
        public class Customer : ITableEntity
        {
            // Property for business logic (e.g., display)
                public string CustomerId { get; set; }

                public string Name { get; set; }

                public string Email { get; set; }

            // Properties required by ITableEntity
                public string PartitionKey { get; set; }
                public string RowKey { get; set; }
                public DateTimeOffset? Timestamp { get; set; }
                public ETag ETag { get; set; }
        }
    }
