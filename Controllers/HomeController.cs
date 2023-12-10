using EnergieEros.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;





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

        private static SemaphoreSlim semaphore = new SemaphoreSlim(1); // Semaphore for controlling concurrency

        [Route("api/chat")]
        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatMessage message, [FromQuery] int limit = 10)
        {
            try
            {
                await semaphore.WaitAsync(); // Wait until allowed to proceed (or wait in a queue)

                var apiKey = "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"; // Replace with your OpenAI API key
                var requestContent = new
                {
                    prompt = message?.Content,
                    // Add any other parameters required by OpenAI API
                };
                var json = JsonSerializer.Serialize(requestContent);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var response = await client.PostAsync("https://api.openai.com/v1/engines/davinci/completions", stringContent);
                var responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseString}");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error processing request to OpenAI API");
                }

                
                var responseObject = JsonSerializer.Deserialize<OpenAIResponse>(responseString);

                return Ok(new { replies = responseObject });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, "Internal server error");
            }
            finally
            {
                semaphore.Release(); // Release semaphore to allow the next request
            }
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

            string role;

            if (model.IsAdmin)
            {
                System.Diagnostics.Debug.WriteLine("Admin registration");
                role = "Admin";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Customer registration");
                role = "Customer";
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Role = role, ReversiblePassword = model.Password };
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