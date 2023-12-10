using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnergieEros.Models;
using EnergieEros.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnergieEros.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public AdminController(IOrderService orderService, IProductService productService, IUserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Admin/AdminPanel.cshtml");
        }

        [HttpGet("/api/orders")]
        public async Task<IActionResult> GetOrders()
        {
            IEnumerable<Order> orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("/api/products")]
        public async Task<IActionResult> GetProducts()
        {
            IEnumerable<Product> products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("/api/users")]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<ApplicationUser> users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("/api/orders/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            Order order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        [HttpGet("/api/products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            Product product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpGet("/api/users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            ApplicationUser user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPut("/api/orders/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Order is null");
            }

            await _orderService.UpdateOrderAsync(order);

            return Ok(order);
        }

        [Route("api/products/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null");
            }

            await _productService.UpdateProductAsync(product);

            return Ok(product);
        }

        [Route("api/users/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser user)
        {
            if (user == null)
            {
                return BadRequest("User is null");
            }

            await _userService.UpdateUserAsync(user);

            return Ok(user);
        }

        [Route("api/orders/delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);

            return Ok("Order deleted successfully.");
        }

        [Route("api/products/delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);

            return Ok("Product deleted successfully.");
        }

        [Route("api/users/delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);

            return Ok("User deleted successfully.");
        }
    }
}
