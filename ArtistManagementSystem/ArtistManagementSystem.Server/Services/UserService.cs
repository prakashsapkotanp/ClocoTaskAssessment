using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;

namespace ArtistManagementSystem.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService; // Reusing for CreateUser (hashing/role logic)

        public UserService(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<(List<UserModel> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize)
        {
            return await _userRepository.GetUsersPagedAsync(page, pageSize);
        }

        public async Task<AuthResponseDTO> CreateUserAsync(UserRegisterDto dto)
        {
            var user = new UserModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Dob = dto.Dob,
                Gender = Enum.Parse<Gender>(dto.Gender, true)
            };

            return await _authService.RegisterAsync(user, dto.Role);
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto)
        {
            return await _userRepository.UpdateUserAsync(id, dto);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }
    }
}
