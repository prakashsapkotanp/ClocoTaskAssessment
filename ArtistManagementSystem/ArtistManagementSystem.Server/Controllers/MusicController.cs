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
        public async Task<IActionResult> Create(int artistId, [FromBody] MusicModel model)
        {
            model.ArtistId = artistId;
            var id = await _service.CreateMusicAsync(model);
            return Ok(new { Message = "Song added successfully", Id = id });
        }

        [Authorize(Roles = "super_admin,artist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int artistId, int musicId, [FromBody] MusicModel model)
        {
            model.Id = musicId;
            model.ArtistId = artistId;
            return await _service.UpdateMusicAsync(model) ? Ok() : BadRequest();
        }

        [Authorize(Roles = "super_admin,artist")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _service.DeleteMusicAsync(id) ? Ok() : BadRequest();
        }
    }
}
