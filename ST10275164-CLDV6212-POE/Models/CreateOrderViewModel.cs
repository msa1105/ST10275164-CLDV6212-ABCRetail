using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ST10275164_CLDV6212_POE.Models
{
    public class CreateOrderViewModel
    {
        public Order Order { get; set; } = new Order();
        public SelectList? Customers { get; set; }
        public SelectList? Products { get; set; }
    }
}