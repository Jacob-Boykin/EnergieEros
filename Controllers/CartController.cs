using EnergieEros.Models;
using EnergieEros.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[Route("api/cart")]
[ApiController]
public class CartApiController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartApiController> _logger;

    public CartApiController(ICartService cartService, ILogger<CartApiController> logger)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("items/{UserId}")]
    public async Task<ActionResult<List<CartItem>>> GetCartItems(string UserId)
    {
        _logger.LogInformation("Fetching cart items for User ID: {UserId}", UserId);
        var cartItems = await _cartService.GetCartItemsAsync(UserId);

        // No need to check if cartItems is null or empty - return the list directly
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

    [HttpDelete("remove/{cartItemId}")]
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

    [HttpPost("checkout/{UserId}")]
    public async Task<ActionResult> Checkout(string UserId)
    {
        try
        {
            await _cartService.CheckoutAsync(UserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during checkout for User ID {UserId}: {ExceptionMessage}", UserId, ex.Message);
            return StatusCode(500, "Error during checkout process");
        }
    }

    [HttpGet("total/{UserId}")]
    public async Task<ActionResult<decimal>> GetTotal(string UserId)
    {
        var total = await _cartService.GetTotalAsync(UserId);

        return total;
    }

    [HttpPut("update/{UserId}")]
    public async Task<ActionResult> UpdateCartItem(int cartItemId, CartItem cartItem)
    {
        if (cartItem == null)
        {
            return BadRequest("Cart item is null");
        }

        await _cartService.UpdateCartItemAsync(cartItemId, cartItem);

        return NoContent();
    }
}