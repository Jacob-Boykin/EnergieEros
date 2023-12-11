using EnergieEros.Models;
using EnergieEros.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/cart")]
[ApiController]
public class CartApiController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartApiController(ICartService cartService)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
    }

    [Route("items/{UserId}")]
    [HttpGet]
    public async Task<ActionResult<List<CartItem>>> GetCartItems(string UserId)
    {
        var cartItems = await _cartService.GetCartItemsAsync(UserId);

        if (cartItems == null || !cartItems.Any() || cartItems.Count() == 0)
        {
            return NotFound("No items in the cart");
        }

        return cartItems.ToList();
    }

    [HttpPost("add/{UserId}")]
    public async Task<ActionResult<CartItem>> AddCartItem([FromBody] CartItem cartItem)
    {
        if (cartItem == null)
        {
            return BadRequest("Cart item is null");
        }

        await _cartService.AddCartItemAsync(cartItem);

        return CreatedAtAction(nameof(GetCartItems), cartItem);
    }

    [HttpDelete("remove/{UserId}")]
    public async Task<ActionResult> RemoveCartItem(int cartItemId)
    {
        await _cartService.RemoveCartItemAsync(cartItemId);

        return NoContent();
    }

    [HttpDelete("clear/{UserId}")]
    public async Task<ActionResult> ClearCart(string UserId)
    {
        await _cartService.ClearCartAsync(UserId);

        return NoContent();
    }

    [HttpGet("checkout/{UserId}")]
    public async Task<ActionResult> Checkout(string UserId)
    {
        await _cartService.CheckoutAsync(UserId);

        return NoContent();
    }

    [HttpGet("total/{UserId}")]
    public async Task<ActionResult<decimal>> GetTotal(string UserId)
    {
        var total = await _cartService.GetTotalAsync(UserId);

        return total;
    }
}