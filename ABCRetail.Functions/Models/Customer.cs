using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Functions.Models
{
    public class Customer
    {
        // This will become the auto-incrementing Primary Key in SQL
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        // This property was being sent by your web app's CustomersController
        public string Address { get; set; } = string.Empty;
    }
}