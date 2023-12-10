using EnergieEros.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.DiaSymReader;
using System.Data.Entity;



namespace EnergieEros.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(
            ILogger<HomeController> logger,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Redirect authenticated users to the appropriate page
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    // Handle account lockout
                    ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
                }
                else
                {
                    // Set specific error message for incorrect username or password
                    ViewData["UsernameError"] = "Either incorrect username or password. Please try again.";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your username and password.");
                }
            }

            // Return to the login view with errors
            return View("~/Views/Account/Login.cshtml", model);
        }

        [Route("api/chat")]
        [HttpPost]
        public IActionResult Chat([FromBody] ChatMessage message)
        {
            // Process the message here (e.g., interact with OpenAI API)
            var replyMessage = $"Received: {message?.Content}";

            return Json(new { reply = replyMessage });
        }

        public IActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                    }
                }
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assigning a role to the user
                if (model.IsAdmin) 
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                     await _userManager.AddToRoleAsync(user, "Customer");
                }

                // Handle successful registration
                System.Diagnostics.Debug.WriteLine("User created a new account with password.");
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login", "Home");
            }
            else {
                // Handle registration failure
                foreach (var error in result.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.Description);
                    TempData["ErrorMessage"] += error.Description;
                }
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View("~/Views/Account/Register.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Redirect to the home page
            return RedirectToAction("Index", "Home");
        }

        [HttpDelete("api/users/delete-all")]
        public async Task<IActionResult> DeleteAllUsers()
        {
            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to delete users.");
                }
            }

            return Ok("All users deleted successfully.");
        }

        [Route("api/viewusers")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var allUsers = _userManager.Users.ToList();
            return Ok(allUsers);
        }

        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        public IActionResult AI()
        {
            return View("~/Views/AI/AI.cshtml");
        }

        public IActionResult AdminPanel()
        {
            return View("~/Views/Admin/AdminPanel.cshtml");
        }

        public IActionResult UserPanel()
        {
            return View("~/Views/User/UserProfile.cshtml");
        }

        public IActionResult Orders()
        {
            return View("~/Views/Orders/Orders.cshtml");
        }

        public IActionResult Cart()
        {
            return View("~/Views/Orders/Cart.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("api/user/id")]
        public async Task<IActionResult> GetUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                System.Diagnostics.Debug.WriteLine($"User ID: {user.Id}");
                return Content($"User ID: {user.Id}");
            }

            return Content("User not found");
        }

    }

    public class ChatMessage
    {
        public string Content { get; set; }
    }
}