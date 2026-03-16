using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<bool> RegisterUserAsync(UserModel user, string roleName);
        Task<bool> UserExistsAsync(string email);
    }
}
