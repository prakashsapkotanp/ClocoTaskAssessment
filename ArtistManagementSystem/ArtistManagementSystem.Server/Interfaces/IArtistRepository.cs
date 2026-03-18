using ArtistManagementSystem.Server.Models;

namespace ArtistManagementSystem.Server.Interfaces
{
    public interface IArtistRepository
    {
        Task<(List<ArtistModel> Artists, int TotalCount)> GetArtistsPagedAsync(int page, int pageSize);
        Task<ArtistModel?> GetArtistByIdAsync(int id);
        Task<int> CreateArtistAsync(ArtistModel artist);
        Task<bool> UpdateArtistAsync(ArtistModel artist);
        Task<bool> DeleteArtistAsync(int id);
        Task<List<ArtistModel>> GetAllArtistsAsync();
        Task<List<ArtistModel>> SearchArtistsByNameAsync(string name);
        Task<ArtistModel?> GetArtistByUserIdAsync(int userId);
    }
}
