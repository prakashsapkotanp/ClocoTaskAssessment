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
        public MusicController(IMusicService service) => _service = service;

        [Authorize(Roles = "super_admin,artist_manager,artist")]
        [HttpGet]
        public async Task<IActionResult> GetByArtist(int artistId)
        {
            return Ok(await _service.GetMusicByArtistAsync(artistId));
        }

        [Authorize(Roles = "super_admin,artist")]
        [HttpPost]
        public async Task<IActionResult> Create(int artistId, [FromBody] MusicDto dto)
        {
            var id = await _service.CreateMusicAsync(artistId, dto);
            return Ok(new { Message = "Song added successfully", Id = id });
        }

        [Authorize(Roles = "super_admin,artist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int artistId, int id, [FromBody] MusicDto dto)
        {
            return await _service.UpdateMusicAsync(artistId, id, dto) ? Ok() : BadRequest();
        }

        [Authorize(Roles = "super_admin,artist")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _service.DeleteMusicAsync(id) ? Ok() : BadRequest();
        }
    }
}
