using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtistManagementSystem.Server.Controllers
{
    [Authorize(Roles = "super_admin")] // olny super_admin can manage users later we will use policies
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1) return BadRequest("Invalid pagination parameters.");

            var (users, totalCount) = await _userService.GetUsersPagedAsync(page, pageSize);

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = users
            });
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRegisterDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);

            if (result.Status == "Failure") return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO dto)
        {
            var success = await _userService.UpdateUserAsync(id, dto);
            if (!success) return NotFound(new { message = "User not found or update failed." });

            return Ok(new { message = "User updated successfully." });
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound(new { message = "User not found." });

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
