using Api_Project.Models;

public interface IUserRepository
{
    Task<ApplicationUser> GetUserByIdAsync(string id);
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(ApplicationUser user);
}
