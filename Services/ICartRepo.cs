using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;
using EnergieEros.Data;
using EnergieEros.Models;
using EnergieEros.Services;
using Microsoft.EntityFrameworkCore;

public class CartRepository : ICartRepository
{
    private readonly EnergieDbContext _context;
    private readonly OrderDbContext _orderDbContext;

    public CartRepository(EnergieDbContext energieDbContext, OrderDbContext orderDbContext)
    {
        _context = energieDbContext;
        _orderDbContext = orderDbContext;
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
    {
        return await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
    }

    public async Task AddCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveCartItemAsync(int cartItemId)
    {
        var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var cartItems = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalAsync(string userId)
    {
        var total = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Join(
                _context.Products,
                cartItem => cartItem.ProductId,
                product => product.ProductId,
                (cartItem, product) => new { cartItem, product }
            )
            .Select(joined => joined.cartItem.Quantity * joined.product.Price)
            .SumAsync();

        return total;
    }

    public async Task<int> GetCountAsync(string userId)
    {
        var count = await _context.CartItems.Where(c => c.UserId == userId).Select(c => c.Quantity).SumAsync();
        return count;
    }

    public async Task<bool> CheckoutAsync(string userId)
    {
        var cartItems = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        var orderProducts = new List<OrderProduct>();
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalAmount = await GetTotalAsync(userId)
        };

        // Add order to the database and get the generated OrderId
        var generatedOrderId = await _orderDbContext.AddOrderAsync(order);

        foreach (var cartItem in cartItems)
        {
            var orderProduct = new OrderProduct
            {
                ProductId = cartItem.ProductId,
                OrderId = generatedOrderId ?? 0, // Use the generated OrderId
            };
            orderProducts.Add(orderProduct);
        }

        // Update the order with the order products
        order.OrderProducts = orderProducts;
        await _orderDbContext.SaveChangesAsync();

        return true; // Assuming success means the order was created and products were added
    }

}
