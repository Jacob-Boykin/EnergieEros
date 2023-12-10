using EnergieEros.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EnergieEros.Data
{
    public class EnergieDbContext : IdentityDbContext<ApplicationUser>
    {
        public EnergieDbContext(DbContextOptions<EnergieDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configurations: specifying relationships, seeding initial data, etc.
        }
    }
}
