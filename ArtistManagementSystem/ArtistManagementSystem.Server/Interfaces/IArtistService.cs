using ArtistManagementSystem.Server.Models;
using Microsoft.AspNetCore.Http;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IArtistService
    {
        Task<(List<ArtistModel> Artists, int TotalCount)> GetArtistsAsync(int page, int pageSize);
        Task<int> CreateArtistAsync(ArtistModel artist);
        Task<bool> UpdateArtistAsync(ArtistModel artist);
        Task<bool> DeleteArtistAsync(int id);
        Task<byte[]> ExportToCsvAsync();
        Task<int> ImportFromCsvAsync(IFormFile file);
    }
}
