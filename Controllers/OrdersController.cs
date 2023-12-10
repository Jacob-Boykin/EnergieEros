using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnergieEros.Data;
using EnergieEros.Models;
using EnergieEros.Services;
using Microsoft.AspNetCore.SignalR;

namespace EnergieEros.Controllers
{
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

    [Route("api/products")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsApiController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            if (products == null || !products.Any() || products.Count() == 0)
            {
                return NotFound("No products found");
            }

            return products.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return product;
        }

        [Route("add")]
        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null");
            }

            await _productService.AddProductAsync(product);

            return CreatedAtAction(nameof(GetProductById), product);
        }

        [Route("update")]
        [HttpPut]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null");
            }

            await _productService.UpdateProductAsync(product);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);

            return NoContent();
        }
    }
}
