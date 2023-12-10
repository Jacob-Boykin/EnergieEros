using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EnergieEros.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public CartItem() { }

        public CartItem(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public override string ToString()
        {
               return $"CartItem: Id={UserId}, ProductId={ProductId}, Quantity={Quantity}";
        }
        // Additional properties if needed
    }

    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd(); // Configure auto-generated order IDs
        }

        public async Task<int?> AddOrderAsync(CartItem item)
        {
            try
            {
                CartItems.Add(item);
                await SaveChangesAsync();
                return item.Id; // Return the generated OrderId
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                Console.WriteLine($"Error adding order: {ex.Message}");
                return null;
            }
        }

    }
}
