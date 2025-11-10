using System.ComponentModel.DataAnnotations;

namespace ST10275164_CLDV6212_POE.Models
{
    public class Customer
    {
        // This will be the new Primary Key
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        // This property is sent by your CustomersController
        public string Address { get; set; } = string.Empty;
    }
}