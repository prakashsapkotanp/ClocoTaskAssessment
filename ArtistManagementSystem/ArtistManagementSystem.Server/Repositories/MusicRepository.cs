using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ArtistManagementSystem.Server.Repositories
{
    public class MusicRepository : IMusicRepository
    {
        private readonly string _connectionString;
        public MusicRepository(IConfiguration config) =>
            _connectionString = config.GetConnectionString("DefaultConnection")!;

        public async Task<List<MusicModel>> GetMusicByArtistAsync(int artistId)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Music WHERE ArtistId = @ArtistId ORDER BY CreatedAt DESC";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ArtistId", artistId);

            var list = new List<MusicModel>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapMusic(reader));
            return list;
        }

        public async Task<int> CreateMusicAsync(MusicModel m)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = @"INSERT INTO Music (ArtistId, Title, AlbumName, Genre, CreatedAt, FilePath) 
                            VALUES (@AId, @Title, @Album, @Genre, @CA, @FPath); SELECT SCOPE_IDENTITY();";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            AddParams(cmd, m);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateMusicAsync(MusicModel m)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = @"UPDATE Music SET Title=@Title, AlbumName=@Album, Genre=@Genre, UpdatedAt=@UA, FilePath=@FPath 
                            WHERE Id=@Id";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", m.Id);
            AddParams(cmd, m);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteMusicAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("DELETE FROM Music WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<MusicModel?> GetMusicByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT * FROM Music WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapMusic(reader) : null;
        }

        private static void AddParams(SqlCommand cmd, MusicModel m)
        {
            cmd.Parameters.AddWithValue("@AId", m.ArtistId);
            cmd.Parameters.AddWithValue("@Title", m.Title);
            cmd.Parameters.AddWithValue("@Album", (object?)m.AlbumName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Genre", m.Genre.ToString());
            cmd.Parameters.AddWithValue("@CA", DateTime.Now);
            cmd.Parameters.AddWithValue("@UA", DateTime.Now);
            cmd.Parameters.AddWithValue("@FPath", (object?)m.FilePath ?? DBNull.Value);
        }

        private static MusicModel MapMusic(SqlDataReader r) => new()
        {
            Id = (int)r["Id"],
            ArtistId = (int)r["ArtistId"],
            Title = r["Title"].ToString()!,
            AlbumName = r["AlbumName"] as string,
            Genre = Enum.Parse<Genre>(r["Genre"].ToString()!),
            CreatedAt = (DateTime)r["CreatedAt"],
            UpdatedAt = r["UpdatedAt"] as DateTime?,
            FilePath = r["FilePath"] != DBNull.Value ? r["FilePath"].ToString() : null
        };
    }
}
