using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;
using EnergieEros.Data;
using EnergieEros.Models;
using EnergieEros.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

public class CartRepository : ICartRepository
{
    private readonly EnergieDbContext _context;
    private readonly ILogger<CartRepository> _logger;   
    private readonly IOrderService _orderService;

    public CartRepository(EnergieDbContext energieDbContext, IOrderService orderService, ILogger<CartRepository> logger)
    {
        _context = energieDbContext;
        _orderService = orderService;
        _logger = logger;
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
        try
        {
            var cartItems = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            if (cartItems.Count == 0)
            {
                // No items in the cart to checkout
                return false;
            }

            var totalAmount = await GetTotalAsync(userId);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount
            };

            // Use the order service to add the order
            await _orderService.AddOrderAsync(order);

            // Clear the cart items
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError("Checkout failed for user {UserId}: {Exception}", userId, ex);

            return false;
        }
    }


    public async Task UpdateCartItem(int cartItemId, CartItem cartItem)
    {
        var cartItemToUpdate = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);
        if (cartItemToUpdate != null)
        {
            cartItemToUpdate.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();
        }
    }
}
