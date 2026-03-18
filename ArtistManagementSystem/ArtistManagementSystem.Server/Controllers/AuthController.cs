using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ArtistManagementSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto )
        {
            var userModel = new UserModel
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                Dob = registerDto.Dob,
                Gender = Enum.Parse<Gender>(registerDto.Gender, true)
            };

            var result = await _authService.RegisterAsync(userModel, registerDto.Role);

            if (result.Status == "Failure")
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestDTO loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            if (response.Status == "Failure")
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}