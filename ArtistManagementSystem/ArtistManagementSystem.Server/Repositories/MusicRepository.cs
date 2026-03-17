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
            while (await reader.ReadAsync())
            {
                list.Add(new MusicModel
                {
                    Id = (int)reader["Id"],
                    ArtistId = (int)reader["ArtistId"],
                    Title = reader["Title"].ToString()!,
                    AlbumName = reader["AlbumName"]?.ToString(),
                    Genre = Enum.Parse<Genre>(reader["Genre"].ToString()!),
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    UpdatedAt = reader["UpdatedAt"] as DateTime?
                });
            }
            return list;
        }

        public async Task<int> CreateMusicAsync(MusicModel m)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = @"INSERT INTO Music (ArtistId, Title, AlbumName, Genre, CreatedAt) 
                            VALUES (@AId, @Title, @Album, @Genre, @CA); SELECT SCOPE_IDENTITY();";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AId", m.ArtistId);
            cmd.Parameters.AddWithValue("@Title", m.Title);
            cmd.Parameters.AddWithValue("@Album", (object?)m.AlbumName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Genre", m.Genre.ToString());
            cmd.Parameters.AddWithValue("@CA", DateTime.Now);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateMusicAsync(MusicModel m)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = @"UPDATE Music SET Title=@Title, AlbumName=@Album, Genre=@Genre, UpdatedAt=@UA 
                            WHERE Id=@Id";
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", m.Id);
            cmd.Parameters.AddWithValue("@Title", m.Title);
            cmd.Parameters.AddWithValue("@Album", (object?)m.AlbumName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Genre", m.Genre.ToString());
            cmd.Parameters.AddWithValue("@UA", DateTime.Now);
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
            if (await reader.ReadAsync())
            {
                return new MusicModel
                {
                    Id = (int)reader["Id"],
                    ArtistId = (int)reader["ArtistId"],
                    Title = reader["Title"].ToString()!,
                    Genre = Enum.Parse<Genre>(reader["Genre"].ToString()!)
                };
            }
            return null;
        }
    }
}
