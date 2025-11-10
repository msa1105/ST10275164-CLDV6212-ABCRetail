using ABCRetail.Functions.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCRetail.Functions
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // These DbSet properties map to tables in your SQL database
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; } // <-- ADD THIS
        public DbSet<Order> Orders { get; set; }   // <-- ADD THIS
    }
}