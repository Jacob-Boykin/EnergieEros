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
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;

namespace EnergieEros.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(IOrderService orderService, IProductService productService, IUserService userService, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, ILogger<AdminController> logger, SignInManager<ApplicationUser> signInManager)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
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
        _logger.LogInformation("Updating user with ID: {UserId}", userId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid. Errors: {ModelStateErrors}", ModelState);
            return BadRequest(ModelState);
        }

        if (user == null)
        {
            _logger.LogWarning("Received a null user object for update.");
            return BadRequest("User is null");
        }

        var existingUser = await _userManager.FindByIdAsync(userId);
        if (existingUser == null)
        {
            _logger.LogWarning("User with ID: {UserId} not found.", userId);
            return NotFound("User not found");
        }

        existingUser.Email = user.Email;
        existingUser.Role = user.Role;
        existingUser.ReversiblePassword = user.ReversiblePassword;
        existingUser.UserName = user.Email;

        if (!string.IsNullOrWhiteSpace(user.ReversiblePassword))
        {
            _logger.LogInformation("Updating password for user ID: {UserId}", userId);

            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var passwordResult = await _userManager.ResetPasswordAsync(existingUser, token, user.ReversiblePassword);
            if (!passwordResult.Succeeded)
            {
                _logger.LogWarning("Password update failed for user ID: {UserId}. Errors: {Errors}", userId, passwordResult.Errors);
                return StatusCode(500, "Error updating the password");
            }
        }

            // Check if the role needs to be updated
            var currentRoles = await _userManager.GetRolesAsync(existingUser);

            // If current a user is an admin and the new role is not admin, if they are logged in, log them out immediately
            if (currentRoles.Contains("Admin") && user.Role != "Admin")
            {
                var signInResult = await _signInManager.PasswordSignInAsync(existingUser, user.ReversiblePassword, false, false);
                if (signInResult.Succeeded)
                {
                    // Log them out and redirect them to the home page
                    _logger.LogInformation("Admin user with ID: {UserId} is being logged out because their role is being changed.", userId);
                    await _signInManager.SignOutAsync();
                }
            }

            if (!currentRoles.Contains(user.Role))
            {
                // Remove all current roles
                var roleRemovalResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                if (!roleRemovalResult.Succeeded)
                {
                    _logger.LogWarning("Failed to remove roles for user ID: {UserId}. Errors: {Errors}", userId, roleRemovalResult.Errors);
                    return StatusCode(500, "Error updating roles");
                }

                // Add the new role
                var roleAdditionResult = await _userManager.AddToRoleAsync(existingUser, user.Role);
                if (!roleAdditionResult.Succeeded)
                {
                    _logger.LogWarning("Failed to add role for user ID: {UserId}. Errors: {Errors}", userId, roleAdditionResult.Errors);
                    return StatusCode(500, "Error updating roles");
                }
            }

            var result = await _userManager.UpdateAsync(existingUser);
        if (!result.Succeeded)
        {
            _logger.LogWarning("User update failed for user ID: {UserId}. Errors: {Errors}", userId, result.Errors);
            return StatusCode(500, "A problem happened while handling your request.");
        }

        _logger.LogInformation("User with ID: {UserId} updated successfully.", userId);
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

        [Route("admin/reports/{reportType}")]
        [HttpGet]
        public async Task<IActionResult> GetReport(string reportType)
        {
            if (reportType == "userActivity") // User activity report
            {
                var allUsers = await _userService.GetAllUsersAsync();
                var allOrders = await _orderService.GetAllOrdersAsync();

                var report = allUsers.Select(user =>
                {
                    var userOrders = allOrders.Where(o => o.UserId == user.Id).ToList();
                    return new
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        OrdersCount = userOrders.Count,
                        TotalSpent = userOrders.Sum(o => o.TotalAmount)
                    };
                }).ToList();

                return Ok(report);
            }
            else if (reportType == "salesSummary") // Sales summary report
            {
                var allOrders = await _orderService.GetAllOrdersAsync();

                var report = allOrders.Select(order => new
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalAmount
                }).ToList();

                return Ok(report);
            }
            else
            {
                return BadRequest("Invalid report type");
            }
        }
    }
}
