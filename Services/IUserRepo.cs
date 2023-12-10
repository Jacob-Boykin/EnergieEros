using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnergieEros.Data;
using EnergieEros.Models;
using EnergieEros.Services;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly EnergieDbContext _context;

    public UserRepository(EnergieDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<ApplicationUser> GetUserByIdAsync(string userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task AddUserAsync(ApplicationUser user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }   

    public async Task AssignAdminRoleAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = "Admin";
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnassignAdminRoleAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = "User";
            await _context.SaveChangesAsync();
        }
    }
}
