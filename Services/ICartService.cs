using System.Collections.Generic;
using System.Threading.Tasks;
using EnergieEros.Models;
using EnergieEros.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public Task<IEnumerable<CartItem>> GetCartItemsAsync(string Id)
    {
        return _cartRepository.GetCartItemsAsync(Id);
    }

    public Task AddCartItemAsync(CartItem cartItem)
    {
        return _cartRepository.AddCartItemAsync(cartItem);
    }

    public Task RemoveCartItemAsync(int cartItemId)
    {
        return _cartRepository.RemoveCartItemAsync(cartItemId);
    }

    public Task ClearCartAsync(string Id)
    {
        return _cartRepository.ClearCartAsync(Id);
    }

    public Task<decimal> GetTotalAsync(string Id)
    {
        return _cartRepository.GetTotalAsync(Id);
    }

    public Task<int> GetCountAsync(string Id)
    {
        return _cartRepository.GetCountAsync(Id);
    }

    public Task<bool> CheckoutAsync(string Id)
    {
        return _cartRepository.CheckoutAsync(Id);
    }

    public Task UpdateCartItemAsync(int cartItemId, CartItem cartItem)
    {
        return _cartRepository.UpdateCartItem(cartItemId, cartItem);
    }
}
