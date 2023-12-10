using System.Collections.Generic;
using System.Threading.Tasks;
using EnergieEros.Models;
using EnergieEros.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<ApplicationUser> GetUserByIdAsync(string userId)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task AddUserAsync(ApplicationUser user)
    {
        await _userRepository.AddUserAsync(user);
    }

    public async Task UpdateUserAsync(ApplicationUser user)
    {
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task DeleteUserAsync(string userId)
    {
        await _userRepository.DeleteUserAsync(userId);
    }

    public async Task AssignAdminRoleAsync(string userId)
    {
        await _userRepository.AssignAdminRoleAsync(userId);
    }

    public async Task UnassignAdminRoleAsync(string userId)
    {
        await _userRepository.UnassignAdminRoleAsync(userId);
    }
}