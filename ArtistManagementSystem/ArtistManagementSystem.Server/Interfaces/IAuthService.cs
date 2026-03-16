
using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(AuthRequestDTO request);
        Task<AuthResponseDTO> RegisterAsync(UserModel user, string roleName);
    }
}
