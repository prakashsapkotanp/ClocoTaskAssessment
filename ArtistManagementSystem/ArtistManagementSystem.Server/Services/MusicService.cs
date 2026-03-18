using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;

namespace ArtistManagementSystem.Server.Services
{
    public class MusicService : IMusicService
    {
        private readonly IMusicRepository _repo;
        public MusicService(IMusicRepository repo) => _repo = repo;

        public Task<List<MusicModel>> GetMusicByArtistAsync(int artistId) => _repo.GetMusicByArtistAsync(artistId);

        public async Task<int> CreateMusicAsync(int artistId, MusicDto dto)
        {
            var music = new MusicModel
            {
                ArtistId = artistId,
                Title = dto.Title,
                AlbumName = dto.AlbumName,
                Genre = Enum.Parse<Genre>(dto.Genre, true),
                FilePath = dto.FilePath,
                CreatedAt = DateTime.UtcNow
            };
            return await _repo.CreateMusicAsync(music);
        }

        public async Task<int> CreateMusicWithFileAsync(int artistId, MusicDto dto, IFormFile file)
        {
            string? filePath = null;
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "music");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                filePath = Path.Combine(uploads, fileName);
                
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                
                filePath = Path.Combine("uploads", "music", fileName); // Store relative path
            }

            var music = new MusicModel
            {
                ArtistId = artistId,
                Title = dto.Title,
                AlbumName = dto.AlbumName,
                Genre = Enum.Parse<Genre>(dto.Genre, true),
                FilePath = filePath,
                CreatedAt = DateTime.UtcNow
            };
            return await _repo.CreateMusicAsync(music);
        }

        public async Task<bool> UpdateMusicAsync(int artistId, int id, MusicDto dto)
        {
            var music = await _repo.GetMusicByIdAsync(id);
            if (music == null || music.ArtistId != artistId) return false;

            music.Title = dto.Title;
            music.AlbumName = dto.AlbumName;
            music.Genre = Enum.Parse<Genre>(dto.Genre, true);
            music.FilePath = dto.FilePath ?? music.FilePath;
            music.UpdatedAt = DateTime.UtcNow;

            return await _repo.UpdateMusicAsync(music);
        }

        public async Task<(byte[] Bytes, string ContentType, string FileName)?> GetMusicFileAsync(int id)
        {
            var music = await _repo.GetMusicByIdAsync(id);
            if (music == null || string.IsNullOrEmpty(music.FilePath)) return null;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", music.FilePath);
            if (!File.Exists(fullPath)) return null;

            var bytes = await File.ReadAllBytesAsync(fullPath);
            return (bytes, "audio/mpeg", Path.GetFileName(music.FilePath));
        }

        public async Task<bool> DeleteMusicAsync(int id)
        {
            var music = await _repo.GetMusicByIdAsync(id);
            if (music != null && !string.IsNullOrEmpty(music.FilePath))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", music.FilePath);
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            return await _repo.DeleteMusicAsync(id);
        }
    }
}
