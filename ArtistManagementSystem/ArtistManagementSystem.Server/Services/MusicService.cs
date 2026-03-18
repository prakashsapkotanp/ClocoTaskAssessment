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

        public Task<int> CreateMusicAsync(int artistId, MusicDto dto)
        {
            var music = new MusicModel
            {
                ArtistId = artistId,
                Title = dto.Title,
                AlbumName = dto.AlbumName,
                Genre = Enum.Parse<Genre>(dto.Genre, true),
                CreatedAt = DateTime.UtcNow
            };
            return _repo.CreateMusicAsync(music);
        }

        public async Task<bool> UpdateMusicAsync(int artistId, int id, MusicDto dto)
        {
            var music = await _repo.GetMusicByIdAsync(id);
            if (music == null || music.ArtistId != artistId) return false;

            music.Title = dto.Title;
            music.AlbumName = dto.AlbumName;
            music.Genre = Enum.Parse<Genre>(dto.Genre, true);
            music.UpdatedAt = DateTime.UtcNow;

            return await _repo.UpdateMusicAsync(music);
        }

        public Task<bool> DeleteMusicAsync(int id) => _repo.DeleteMusicAsync(id);
    }
}
