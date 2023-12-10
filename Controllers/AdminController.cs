using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnergieEros.Models;
using EnergieEros.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Humanizer;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;

namespace EnergieEros.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IOrderService orderService, IProductService productService, IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View("~/Views/Admin/AdminPanel.cshtml");
        }

        [HttpGet("/admin/orders")]
        public async Task<IActionResult> GetOrders()
        {
            IEnumerable<Order> orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("/admin/products")]
        public async Task<IActionResult> GetProducts()
        {
            IEnumerable<Product> products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("/admin/users")]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<ApplicationUser> users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("/admin/orders/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            Order order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        [HttpGet("/admin/products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            Product product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpGet("/admin/user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            ApplicationUser user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine("User not found");
                return NotFound(new { Message = "User not found" });  // Or some other appropriate response
            }
            return Ok(user);
        }

        [HttpPut("/admin/orders/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Order is null");
            }

            await _orderService.UpdateOrderAsync(order);

            return Ok(order);
        }

        [Route("admin/products/{id}")]
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

        [Route("admin/users/{userId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] ApplicationUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user == null)
            {
                return BadRequest("User is null");
            }

            // Fetch the existing user
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Update the properties you want to change
            existingUser.Email = user.Email;
            existingUser.UserName = user.UserName;
            existingUser.Role = user.Role;
            existingUser.ReversiblePassword = user.ReversiblePassword;

            // Update password hash if the password is provided
            if (!string.IsNullOrWhiteSpace(user.ReversiblePassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var passwordResult = await _userManager.ResetPasswordAsync(existingUser, token, user.ReversiblePassword);
                if (!passwordResult.Succeeded)
                {
                    // Handle error in password update
                    // Optionally, add the error details to the response
                    return StatusCode(500, "Error updating the password");
                }
            }

            // Save the changes
            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                // Handle errors
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(existingUser);
        }


        [Route("admin/orders/delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);

            return Ok("Order deleted successfully.");
        }

        [Route("admin/products/delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);

            return Ok("Product deleted successfully.");
        }

        [Route("admin/users/delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);

            return Ok("User deleted successfully.");
        }
    }
}
