using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IMusicRepository
    {
        Task<List<MusicModel>> GetMusicByArtistAsync(int artistId);
        Task<MusicModel?> GetMusicByIdAsync(int id);
        Task<int> CreateMusicAsync(MusicModel music);
        Task<bool> UpdateMusicAsync(MusicModel music);
        Task<bool> DeleteMusicAsync(int id);
    }
}
