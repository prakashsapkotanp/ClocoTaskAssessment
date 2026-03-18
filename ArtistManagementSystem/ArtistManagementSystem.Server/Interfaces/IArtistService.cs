using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Models;
using Microsoft.AspNetCore.Http;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IArtistService
    {
        Task<(List<ArtistModel> Artists, int TotalCount)> GetArtistsAsync(int page, int pageSize);
        Task<int> CreateArtistAsync(ArtistDto dto);
        Task<bool> UpdateArtistAsync(int id, ArtistDto dto);
        Task<bool> DeleteArtistAsync(int id);
        Task<byte[]> ExportToCsvAsync();
        Task<int> ImportFromCsvAsync(IFormFile file);
        Task<ArtistModel?> GetArtistByUserIdAsync(int userId);
    }
}
