using System.Collections.Generic;
using System.Threading.Tasks;
using EnergieEros.Models;

namespace EnergieEros.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task AddUserAsync(ApplicationUser user);
        Task UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(string userId);
        Task AssignAdminRoleAsync(string userId);
        Task UnassignAdminRoleAsync(string userId);
    }

    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task AddUserAsync(ApplicationUser user);
        Task UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(string userId);
        Task AssignAdminRoleAsync(string userId);
        Task UnassignAdminRoleAsync(string userId);
    }
}
