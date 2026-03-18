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

            var count = (int)await new SqlCommand("SELECT COUNT(*) FROM [artist]", conn).ExecuteScalarAsync();

            var sql = @"SELECT * FROM [artist] ORDER BY CreatedAt DESC 
                    OFFSET @Skip ROWS FETCH NEXT @Size ROWS ONLY";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Skip", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@Size", pageSize);

            var list = new List<ArtistModel>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapArtist(reader));

            return (list, count);
        }

        public async Task<int> CreateArtistAsync(ArtistModel a)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO [artist] (Name, Dob, Gender, Address, FirstReleaseYear, NoOfAlbumsReleased, UserId, CreatedAt) 
                    VALUES (@Name, @Dob, @Gender, @Addr, @FRY, @NOA, @UId, @CA); SELECT SCOPE_IDENTITY();";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            AddParams(cmd, a);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateArtistAsync(ArtistModel a)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"UPDATE [artist] SET Name=@Name, Dob=@Dob, Gender=@Gender, Address=@Addr, 
                    FirstReleaseYear=@FRY, NoOfAlbumsReleased=@NOA, UserId=@UId, UpdatedAt=@UA WHERE Id=@Id";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", a.Id);
            cmd.Parameters.AddWithValue("@UA", DateTime.UtcNow);
            AddParams(cmd, a);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteArtistAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("DELETE FROM [artist] WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<ArtistModel>> GetAllArtistsAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM [artist] ORDER BY Name";
            await conn.OpenAsync();
            var list = new List<ArtistModel>();
            using var reader = await new SqlCommand(sql, conn).ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapArtist(reader));
            return list;
        }

        public async Task<ArtistModel?> GetArtistByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM [artist] WHERE Id = @Id";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapArtist(reader) : null;
        }

        public async Task<ArtistModel?> GetArtistByUserIdAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM [artist] WHERE UserId = @UId";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UId", userId);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapArtist(reader) : null;
        }

        public async Task<List<ArtistModel>> SearchArtistsByNameAsync(string name)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM [artist] WHERE Name LIKE @Name + '%' ORDER BY Name";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", name);
            
            var list = new List<ArtistModel>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapArtist(reader));
            return list;
        }

        private static void AddParams(SqlCommand cmd, ArtistModel a)
        {
            cmd.Parameters.AddWithValue("@Name", a.Name);
            cmd.Parameters.AddWithValue("@Dob", a.Dob);
            cmd.Parameters.AddWithValue("@Gender", a.Gender.ToString());
            cmd.Parameters.AddWithValue("@Addr", (object?)a.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FRY", a.FirstReleaseYear);
            cmd.Parameters.AddWithValue("@NOA", a.NoOfAlbumsReleased);
            cmd.Parameters.AddWithValue("@UId", (object?)a.UserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CA", a.CreatedAt == default ? DateTime.UtcNow : a.CreatedAt);
        }

        private static ArtistModel MapArtist(SqlDataReader r) => new()
        {
            Id = (int)r["Id"],
            Name = r["Name"].ToString()!,
            Dob = (DateTime)r["Dob"],
            Gender = Enum.Parse<Gender>(r["Gender"].ToString()!, true),
            Address = r["Address"] as string,
            FirstReleaseYear = (int)r["FirstReleaseYear"],
            NoOfAlbumsReleased = (int)r["NoOfAlbumsReleased"],
            UserId = r["UserId"] as int?
        };
    }
}
