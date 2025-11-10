using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Functions.Models
{
    public class Product
    {
        // This MUST be an int to match the Web App model
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Product Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}