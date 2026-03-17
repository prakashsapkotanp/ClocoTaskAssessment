using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IMusicService
    {
        Task<List<MusicModel>> GetMusicByArtistAsync(int artistId);
        Task<int> CreateMusicAsync(MusicModel music);
        Task<bool> UpdateMusicAsync(MusicModel music);
        Task<bool> DeleteMusicAsync(int id);
    }
}
