using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnergieEros.Data;
using EnergieEros.Models;
using EnergieEros.Services;
using System;
using System.Security.Claims;

namespace EnergieEros
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EnergieDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("EnergieErosContext")));

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartRepository, CartRepository>();

            // Register Identity services for default and custom user/role types
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 3;
                // Use integers for user and role IDs
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.Lockout.AllowedForNewUsers = true;

                // Set integer user ID
                options.User.RequireUniqueEmail = true;
                options.User.RequireUniqueEmail = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<EnergieDbContext>()
            .AddDefaultTokenProviders();

            services.AddControllersWithViews();

            // Clear user sessions on application startup
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnValidatePrincipal = async context =>
                {
                    // Check if the user's security stamp changed (indicating a logout is needed)
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.GetUserAsync(context.Principal);
                    if (user == null || (await userManager.GetSecurityStampAsync(user)) != context.Principal.FindFirstValue("AspNet.Identity.SecurityStamp"))
                    {
                        context.RejectPrincipal(); // Reject the current principal, causing logout
                    }
                };
            });
        }

        public async Task ConfigureAsync(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Ensure user sessions are cleared on application startup
            app.Use(async (context, next) =>
            {
                var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    // Change the user's security stamp to force logout
                    await userManager.UpdateSecurityStampAsync(user);
                }
                await next();
            });

            // Create roles during application startup
            var roles = new[] { "Admin", "User", "Anonymous" };
            var rolesManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in roles)
            {
                if (!await rolesManager.RoleExistsAsync(role))
                {
                    await rolesManager.CreateAsync(new IdentityRole(role));
                }
            }

        }
    }
}
