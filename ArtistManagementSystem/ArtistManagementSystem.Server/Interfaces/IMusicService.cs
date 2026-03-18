using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IMusicService
    {
        Task<List<MusicModel>> GetMusicByArtistAsync(int artistId);
        Task<int> CreateMusicAsync(int artistId, MusicDto dto);
        Task<bool> UpdateMusicAsync(int artistId, int id, MusicDto dto);
        Task<bool> DeleteMusicAsync(int id);
    }
}
