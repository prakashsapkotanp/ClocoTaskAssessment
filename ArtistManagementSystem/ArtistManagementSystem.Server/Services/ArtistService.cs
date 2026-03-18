using ArtistManagementSystem.Server.DTOs;
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
        private readonly IAuthRepository _authRepo;
        public ArtistService(IArtistRepository repo, IAuthRepository authRepo)
        {
            _repo = repo;
            _authRepo = authRepo;
        }

        public async Task<(List<ArtistModel> Artists, int TotalCount)> GetArtistsAsync(int page, int pageSize) =>
            await _repo.GetArtistsPagedAsync(page, pageSize);

        public async Task<int> CreateArtistAsync(ArtistDto dto)
        {

           


            // 1. Create User
            var user = new UserModel
            {
                FirstName = dto.Name,
                LastName = "Artist",
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Dob = dto.Dob,
                Gender = Enum.Parse<Gender>(dto.Gender, true),
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };

            if (await _authRepo.UserExistsAsync(user.Email))
            {
                throw new InvalidOperationException("Email is already registered");
            }
            var userId = await _authRepo.RegisterUserAsync(user, "artist");

            // 2. Create Artist linked to User
            var artist = new ArtistModel
            {
                Name = dto.Name,
                Dob = dto.Dob,
                Gender = Enum.Parse<Gender>(dto.Gender, true),
                Address = dto.Address,
                FirstReleaseYear = dto.FirstReleaseYear,
                NoOfAlbumsReleased = dto.NoOfAlbumsReleased,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            return await _repo.CreateArtistAsync(artist);
        }

        public async Task<bool> UpdateArtistAsync(int id, ArtistDto dto)
        {
            var artist = await _repo.GetArtistByIdAsync(id);
            if (artist == null) return false;

            artist.Name = dto.Name;
            artist.Dob = dto.Dob;
            artist.Gender = Enum.Parse<Gender>(dto.Gender, true);
            artist.Address = dto.Address;
            artist.FirstReleaseYear = dto.FirstReleaseYear;
            artist.NoOfAlbumsReleased = dto.NoOfAlbumsReleased;
            artist.UpdatedAt = DateTime.UtcNow;

            return await _repo.UpdateArtistAsync(artist);
        }

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
            await stream.ReadLineAsync(); // Skip header row

            while (await stream.ReadLineAsync() is string line)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var vals = line.Split(',');
                if (vals.Length < 6) continue;

                // Handle both "Name,Dob..." and "Id,Name,Dob..." formats
                int i = vals.Length == 7 ? 1 : 0; 
                
                try {
                    await _repo.CreateArtistAsync(new ArtistModel {
                        Name = vals[i],
                        Dob = DateTime.Parse(vals[i + 1]),
                        Gender = Enum.Parse<Gender>(vals[i + 2], true),
                        Address = vals[i + 3],
                        FirstReleaseYear = int.Parse(vals[i + 4]),
                        NoOfAlbumsReleased = int.Parse(vals[i + 5]),
                        CreatedAt = DateTime.UtcNow
                    });
                    count++;
                } catch { /* Skip invalid data */ }
            }
            return count;
        }

        public Task<ArtistModel?> GetArtistByUserIdAsync(int userId) => _repo.GetArtistByUserIdAsync(userId);

        public Task<List<ArtistModel>> SearchArtistsByNameAsync(string name) => _repo.SearchArtistsByNameAsync(name);
    }
}
