using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Services
{
    public class MusicService : IMusicService
    {
        private readonly IMusicRepository _repo;
        public MusicService(IMusicRepository repo) => _repo = repo;

        public Task<List<MusicModel>> GetMusicByArtistAsync(int artistId) => _repo.GetMusicByArtistAsync(artistId);
        public Task<int> CreateMusicAsync(MusicModel music) => _repo.CreateMusicAsync(music);
        public Task<bool> UpdateMusicAsync(MusicModel music) => _repo.UpdateMusicAsync(music);
        public Task<bool> DeleteMusicAsync(int id) => _repo.DeleteMusicAsync(id);
    }
}
