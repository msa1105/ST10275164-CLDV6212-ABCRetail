using System;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Functions.Models
{
    public class Order
    {
        // This will be the new auto-incrementing Primary Key
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; } // <-- Changed to int

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; } // <-- Changed to int

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }

        // --- EF Core Navigation Properties ---
        // These link the 'int' IDs above to the actual objects
        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }
    }
}