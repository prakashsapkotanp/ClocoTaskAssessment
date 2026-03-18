using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> DeleteUserAsync(int id);
        Task<(List<UserModel> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto);
        Task<UserModel?> GetUserByEmailAndRoleAsync(string email, RoleType role);
    }
}
