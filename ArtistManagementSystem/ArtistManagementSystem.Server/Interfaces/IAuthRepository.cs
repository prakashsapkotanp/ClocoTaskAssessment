using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<int> RegisterUserAsync(UserModel user, string roleName);
        Task<bool> UserExistsAsync(string email);
    }
}
