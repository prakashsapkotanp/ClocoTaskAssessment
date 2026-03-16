using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> DeleteUserAsync(int id);
        Task<(List<UserModel> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto);
    }
}
