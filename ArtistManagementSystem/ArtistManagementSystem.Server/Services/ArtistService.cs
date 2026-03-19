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
        private readonly IUserRepository _userRepo;
        public ArtistService(IArtistRepository repo, IAuthRepository authRepo, IUserRepository userRepo)
        {
            _repo = repo;
            _authRepo = authRepo;
            _userRepo = userRepo;
        }

        public async Task<(List<ArtistModel> Artists, int TotalCount)> GetArtistsAsync(int page, int pageSize) =>
            await _repo.GetArtistsPagedAsync(page, pageSize);

        public async Task<int> CreateArtistAsync(ArtistDto dto)
        {

           


            // 1. Create User
            var user = new UserModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
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
                Name = $"{dto.FirstName} {dto.LastName}",
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

            artist.Name = $"{dto.FirstName} {dto.LastName}";
            artist.Dob = dto.Dob;
            artist.Gender = Enum.Parse<Gender>(dto.Gender, true);
            artist.Address = dto.Address;
            artist.FirstReleaseYear = dto.FirstReleaseYear;
            artist.NoOfAlbumsReleased = dto.NoOfAlbumsReleased;
            artist.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Password) && artist.UserId > 0)
            {
                var userUpdate = new UserUpdateDTO
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Dob = dto.Dob,
                    Gender = dto.Gender,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };
                await _userRepo.UpdateUserAsync((int)artist.UserId, userUpdate);
            }

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
            var headerLine = await stream.ReadLineAsync(); // Read header row
            if (string.IsNullOrWhiteSpace(headerLine)) throw new ArgumentException("CSV file is empty or missing headers.");

            var headers = headerLine.Split(',').Select(h => h.Trim().ToLower()).ToList();
            var emailIdx = headers.IndexOf("email");
            var passIdx = headers.IndexOf("password");

            if (emailIdx == -1 || passIdx == -1)
                throw new ArgumentException("Email and Password columns are missing in the CSV file.");

            var nameIdx = headers.IndexOf("name");
            var dobIdx = headers.IndexOf("dob");
            var genderIdx = headers.IndexOf("gender");
            var addressIdx = headers.IndexOf("address");
            var fryIdx = headers.IndexOf("firstreleaseyear");
            var albumsIdx = headers.IndexOf("albums");

            if (nameIdx == -1 || dobIdx == -1 || genderIdx == -1 || fryIdx == -1 || albumsIdx == -1)
                throw new ArgumentException("Required columns (Name, Dob, Gender, FirstReleaseYear, Albums) are missing.");

            while (await stream.ReadLineAsync() is string line)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var vals = line.Split(',');
                
                try {
                    var name = vals[nameIdx].Trim();
                    var nameParts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var firstName = nameParts.Length > 0 ? nameParts[0] : "Artist";
                    var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "Unknown";
                    var dob = DateTime.Parse(vals[dobIdx]);
                    var gender = Enum.Parse<Gender>(vals[genderIdx], true);
                    var address = addressIdx != -1 && addressIdx < vals.Length ? vals[addressIdx]?.Trim() : null;
                    var email = emailIdx < vals.Length ? vals[emailIdx].Trim() : "";
                    var password = passIdx < vals.Length ? vals[passIdx].Trim() : "";

                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) continue;

                    if (!email.Contains("@"))
                    {
                        throw new ArgumentException($"Invalid email format: '{email}'. It must contain '@'.");
                    }

                    if (await _authRepo.UserExistsAsync(email))
                    {
                        throw new ArgumentException($"Email '{email}' is already registered. Import aborted.");
                    }

                    var user = new UserModel
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        Phone = null,
                        Password = BCrypt.Net.BCrypt.HashPassword(password),
                        Dob = dob,
                        Gender = gender,
                        Address = address,
                        CreatedAt = DateTime.UtcNow
                    };

                    int userId = await _authRepo.RegisterUserAsync(user, "artist");

                    await _repo.CreateArtistAsync(new ArtistModel {
                        Name = name,
                        Dob = dob,
                        Gender = gender,
                        Address = address,
                        FirstReleaseYear = int.Parse(vals[fryIdx]),
                        NoOfAlbumsReleased = int.Parse(vals[albumsIdx]),
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    });
                    count++;
                } 
                catch (ArgumentException)
                {
                    throw; // Rethrow business validation errors
                }
                catch { /* Skip invalid data from parsing failures */ }
            }
            return count;
        }

        public Task<ArtistModel?> GetArtistByUserIdAsync(int userId) => _repo.GetArtistByUserIdAsync(userId);

        public Task<List<ArtistModel>> SearchArtistsByNameAsync(string name) => _repo.SearchArtistsByNameAsync(name);
    }
}
