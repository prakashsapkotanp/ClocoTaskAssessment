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
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
            Ok(await _service.GetArtistsAsync(page, pageSize));

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArtistModel model) =>
            Ok(await _service.CreateArtistAsync(model));

        [Authorize(Roles = "super_admin,artist_manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArtistModel model)
        {
            model.Id = id;
            return await _service.UpdateArtistAsync(model) ? Ok() : BadRequest();
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
            if (file == null || file.Length == 0) return BadRequest("File is empty");
            var count = await _service.ImportFromCsvAsync(file);
            return Ok(new { Message = $"Imported {count} artists successfully." });
        }
    }
}
