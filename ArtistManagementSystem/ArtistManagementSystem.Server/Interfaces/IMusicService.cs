using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IMusicService
    {
        Task<List<MusicModel>> GetMusicByArtistAsync(int artistId);
        Task<int> CreateMusicAsync(int artistId, MusicDto dto);
        Task<int> CreateMusicWithFileAsync(int artistId, MusicDto dto, IFormFile file);
        Task<bool> UpdateMusicAsync(int artistId, int id, MusicDto dto);
        Task<bool> DeleteMusicAsync(int id);
        Task<(byte[] Bytes, string ContentType, string FileName)?> GetMusicFileAsync(int id);
    }
}
