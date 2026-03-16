using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IUserService
    {
        Task<(List<UserModel> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize);
        Task<AuthResponseDTO> CreateUserAsync(UserRegisterDto dto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto);
        Task<bool> DeleteUserAsync(int id);
    }
}
