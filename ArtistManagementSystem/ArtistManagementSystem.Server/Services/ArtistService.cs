using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace ArtistManagementSystem.Server.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _repo;
        public ArtistService(IArtistRepository repo) => _repo = repo;

        public Task<(List<ArtistModel> Artists, int TotalCount)> GetArtistsAsync(int page, int pageSize) =>
            _repo.GetArtistsPagedAsync(page, pageSize);

        public Task<int> CreateArtistAsync(ArtistModel artist) => _repo.CreateArtistAsync(artist);
        public Task<bool> UpdateArtistAsync(ArtistModel artist) => _repo.UpdateArtistAsync(artist);
        public Task<bool> DeleteArtistAsync(int id) => _repo.DeleteArtistAsync(id);

        public async Task<byte[]> ExportToCsvAsync()
        {
            var artists = await _repo.GetAllArtistsAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,Dob,Gender,Address,FirstReleaseYear,Albums");
            foreach (var a in artists)
                csv.AppendLine($"{a.Id},{a.Name},{a.Dob:yyyy-MM-dd},{a.Gender},{a.Address},{a.FirstReleaseYear},{a.NoOfAlbumsReleased}");
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<int> ImportFromCsvAsync(IFormFile file)
        {
            int count = 0;
            using var stream = new StreamReader(file.OpenReadStream());
            var header = await stream.ReadLineAsync(); // Skip header
            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var values = line.Split(',');
                if (values.Length >= 6)
                {
                    int offset = (values.Length == 7) ? 1 : 0; // If Id is present, shift offset by 1
                    
                    try {
                        var artist = new ArtistModel
                        {
                            Name = values[offset],
                            Dob = DateTime.Parse(values[offset + 1]),
                            Gender = Enum.Parse<Gender>(values[offset + 2], true),
                            Address = values[offset + 3],
                            FirstReleaseYear = int.Parse(values[offset + 4]),
                            NoOfAlbumsReleased = int.Parse(values[offset + 5]),
                            CreatedAt = DateTime.UtcNow
                        };
                        await _repo.CreateArtistAsync(artist);
                        count++;
                    } catch {
                        // Skip invalid rows
                    }
                }
            }
            return count;
        }
    }
}
