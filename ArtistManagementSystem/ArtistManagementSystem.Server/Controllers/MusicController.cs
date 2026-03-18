using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtistManagementSystem.Server.Controllers
{
    [Authorize]
    [Route("api/artist/{artistId}/music")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly IMusicService _service;
        private readonly IArtistService _artistService;
        public MusicController(IMusicService service, IArtistService artistService)
        {
            _service = service;
            _artistService = artistService;
        }

        [Authorize(Roles = "super_admin,artist_manager,artist")]
        [HttpGet]
        public async Task<IActionResult> GetByArtist(int artistId)
        {
            if (User.IsInRole("artist"))
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                                  ?? User.FindFirst("id")?.Value;
                if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
                {
                    var artist = await _artistService.GetArtistByUserIdAsync(userId);
                    if (artist == null || artist.Id != artistId)
                    {
                        return Forbid("You can only access your own songs.");
                    }
                }
            }
            return Ok(await _service.GetMusicByArtistAsync(artistId));
        }

        [Authorize(Roles = "super_admin,artist_manager,artist")]
        [HttpPost]
        public async Task<IActionResult> Create(int artistId, [FromForm] MusicDto dto, IFormFile? file)
        {
            if (User.IsInRole("artist"))
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                                  ?? User.FindFirst("id")?.Value;
                if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
                {
                    var artist = await _artistService.GetArtistByUserIdAsync(userId);
                    if (artist == null || artist.Id != artistId)
                    {
                        return Forbid("You can only add songs to your own profile.");
                    }
                }
            }

            int id;
            if (file != null)
                id = await _service.CreateMusicWithFileAsync(artistId, dto, file);
            else
                id = await _service.CreateMusicAsync(artistId, dto);

            return Ok(new { Message = "Song added successfully", Id = id });
        }

        [Authorize(Roles = "super_admin,artist_manager,artist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int artistId, int id, [FromBody] MusicDto dto)
        {
            return await _service.UpdateMusicAsync(artistId, id, dto) ? Ok() : BadRequest();
        }

        [Authorize(Roles = "super_admin,artist_manager,artist")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _service.DeleteMusicAsync(id) ? Ok() : BadRequest();
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _service.GetMusicFileAsync(id);
            if (file == null) return NotFound();
            return File(file.Value.Bytes, file.Value.ContentType, file.Value.FileName);
        }
    }
}
