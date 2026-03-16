using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ArtistManagementSystem.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        public async Task<AuthResponseDTO> LoginAsync(AuthRequestDTO request)
        {
            var user = await _repository.GetUserByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return new AuthResponseDTO { Status = "Failure", Message = "Invalid credentials" };
            }

            return new AuthResponseDTO
            {
                Email = $"{user.FirstName} {user.LastName}",
                Token = GenerateJwtToken(user),
                Status = "Success",
                Message = "Login successful"
            };
        }

        public async Task<AuthResponseDTO> RegisterAsync(UserModel user, string roleName)
        {
            if (await _repository.UserExistsAsync(user.Email))
            {
                return new AuthResponseDTO
                {
                    Status = "Failure",
                    Message = "User with this email already exists.",
                    Email = user.Email
                };
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;


            bool isSaved = await _repository.RegisterUserAsync(user, roleName);

            if (!isSaved)
            {
                return new AuthResponseDTO
                {
                    Status = "Failure",
                    Message = "Database error occurred during registration.",
                    Email = user.Email
                };
            }

            return new AuthResponseDTO
            {
                Email = user.Email,
                Status = "Success",
                Message = "User registered successfully!",
                Token = ""
            };
        }
        private string GenerateJwtToken(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            // Add roles to claims for [Authorize(Roles = "Admin")] to work
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

