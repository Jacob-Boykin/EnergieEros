using System.Linq;
using System.Threading.Tasks;
using EnergieEros.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DataMigration
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DataMigration(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task MigrateUserData()
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var oldUsers = allUsers.Where(u => u.Id.GetType() == typeof(string)).ToList();

        foreach (var oldUser in oldUsers)
        {
            // Remove the old user
            await _userManager.DeleteAsync(oldUser);
        }
    }
}
