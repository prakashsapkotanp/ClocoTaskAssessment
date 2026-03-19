using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtistManagementSystem.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _service;
        public ArtistController(IArtistService service) => _service = service;

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (artists, totalCount) = await _service.GetArtistsAsync(page, pageSize);
            return Ok(new
            {
                Artists = artists,
                TotalCount = totalCount
            });
        }

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArtistDto dto) =>
            Ok(await _service.CreateArtistAsync(dto));

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArtistDto dto)
        {
            return await _service.UpdateArtistAsync(id, dto) ? Ok() : BadRequest();
        }

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteArtistAsync(id) ? Ok() : BadRequest();

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpGet("export")]
        public async Task<IActionResult> Export() =>
            File(await _service.ExportToCsvAsync(), "text/csv", "artists.csv");

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "File is empty" });
            try
            {
                var count = await _service.ImportFromCsvAsync(file);
                return Ok(new { message = $"Imported {count} artists successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name) =>
            Ok(await _service.SearchArtistsByNameAsync(name));

        [Authorize(Roles = "super_admin,artist_manager,artist")]
        [HttpGet("me/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var artist = await _service.GetArtistByUserIdAsync(userId);
            return artist != null ? Ok(artist) : NotFound();
        }
    }
}
