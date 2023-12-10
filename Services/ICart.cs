using System.Collections.Generic;
using System.Threading.Tasks;
using EnergieEros.Models;

namespace EnergieEros.Services
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId);
        Task AddCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearCartAsync(string userId);
        Task<decimal> GetTotalAsync(string id);
        Task<int> GetCountAsync(string id);
        Task<bool> CheckoutAsync(string id);
    }

    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId);
        Task AddCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearCartAsync(string userId);
        Task<decimal> GetTotalAsync(string id);
        Task<int> GetCountAsync(string id);
        Task<bool> CheckoutAsync(string id);
    }
}
