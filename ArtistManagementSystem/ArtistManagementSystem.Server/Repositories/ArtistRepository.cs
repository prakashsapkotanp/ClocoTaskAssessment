using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ArtistManagementSystem.Server.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly string _connectionString;
        public ArtistRepository(IConfiguration configuration) =>
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public async Task<(List<ArtistModel> Artists, int TotalCount)> GetArtistsPagedAsync(int page, int pageSize)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var count = (int)await new SqlCommand("SELECT COUNT(*) FROM Artist", conn).ExecuteScalarAsync();

            var sql = @"SELECT * FROM Artist ORDER BY CreatedAt DESC 
                    OFFSET @Skip ROWS FETCH NEXT @Size ROWS ONLY";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Skip", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@Size", pageSize);

            var list = new List<ArtistModel>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapReader(reader));

            return (list, count);
        }

        public async Task<int> CreateArtistAsync(ArtistModel a)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Artist (Name, Dob, Gender, Address, FirstReleaseYear, NoOfAlbumsReleased, CreatedAt) 
                    VALUES (@Name, @Dob, @Gender, @Addr, @FRY, @NOA, @CA); SELECT SCOPE_IDENTITY();";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            AddParams(cmd, a);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateArtistAsync(ArtistModel a)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"UPDATE Artist SET Name=@Name, Dob=@Dob, Gender=@Gender, Address=@Addr, 
                    FirstReleaseYear=@FRY, NoOfAlbumsReleased=@NOA, UpdatedAt=@UA WHERE Id=@Id";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", a.Id);
            cmd.Parameters.AddWithValue("@UA", DateTime.Now);
            AddParams(cmd, a);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteArtistAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return await new SqlCommand($"DELETE FROM Artist WHERE Id={id}", conn).ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<ArtistModel>> GetAllArtistsAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var list = new List<ArtistModel>();
            using var reader = await new SqlCommand("SELECT * FROM Artist", conn).ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapReader(reader));
            return list;
        }

        private void AddParams(SqlCommand cmd, ArtistModel a)
        {
            cmd.Parameters.AddWithValue("@Name", a.Name);
            cmd.Parameters.AddWithValue("@Dob", a.Dob);
            cmd.Parameters.AddWithValue("@Gender", a.Gender.ToString());
            cmd.Parameters.AddWithValue("@Addr", (object?)a.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FRY", a.FirstReleaseYear);
            cmd.Parameters.AddWithValue("@NOA", a.NoOfAlbumsReleased);
            cmd.Parameters.AddWithValue("@CA", DateTime.Now);
        }

        private ArtistModel MapReader(SqlDataReader r) => new()
        {
            Id = (int)r["Id"],
            Name = r["Name"].ToString()!,
            Dob = (DateTime)r["Dob"],
            Gender = Enum.Parse<Gender>(r["Gender"].ToString()!),
            Address = r["Address"]?.ToString(),
            FirstReleaseYear = (int)r["FirstReleaseYear"],
            NoOfAlbumsReleased = (int)r["NoOfAlbumsReleased"]
        };

        public async Task<ArtistModel?> GetArtistByIdAsync(int id) { /* Implementation similar to MapReader */ return null; }
    }
}
