using EnergieEros.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergieEros.Data
{
    public class EnergieDbContext : DbContext
    {
        public EnergieDbContext(DbContextOptions<EnergieDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        // Other DbSet properties for your models

        // Override methods and configurations as needed
    }
}