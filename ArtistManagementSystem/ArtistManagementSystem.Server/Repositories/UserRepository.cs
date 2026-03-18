using ArtistManagementSystem.Server.DTOs;
using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ArtistManagementSystem.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<(List<UserModel> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // 1. Get Total Count for Pagination UI
            const string countSql = "SELECT COUNT(*) FROM [user]";
            using var countCmd = new SqlCommand(countSql, connection);
            int totalCount = (int)await countCmd.ExecuteScalarAsync();

            // 2. Fetch Paged Data
            int offset = (page - 1) * pageSize;
            const string dataSql = @"
            SELECT * FROM [user] 
            ORDER BY CreatedAt DESC 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using var dataCmd = new SqlCommand(dataSql, connection);
            dataCmd.Parameters.AddWithValue("@Offset", offset);
            dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

            var users = new List<UserModel>();
            using var reader = await dataCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) users.Add(MapUser(reader));

            return (users, totalCount);
        }

        private static UserModel MapUser(SqlDataReader r) => new()
        {
            Id = (int)r["Id"],
            FirstName = r["FirstName"].ToString()!,
            LastName = r["LastName"].ToString()!,
            Email = r["Email"].ToString()!,
            Phone = r["Phone"] != DBNull.Value ? r["Phone"].ToString() : null,
            Address = r["Address"] != DBNull.Value ? r["Address"].ToString() : null,
            Dob = (DateTime)r["Dob"],
            Gender = Enum.Parse<Gender>(r["Gender"].ToString()!, true),
            Role = Enum.Parse<RoleType>(r["Role"].ToString()!, true),
            CreatedAt = (DateTime)r["CreatedAt"]
        };

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            UPDATE [user] 
            SET FirstName = @FN, LastName = @LN, Phone = @Phone, Address = @Addr, Dob = @Dob, Gender = @Gender
            WHERE Id = @Id";

            await connection.OpenAsync();
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@FN", dto.FirstName);
            cmd.Parameters.AddWithValue("@LN", dto.LastName);
            cmd.Parameters.AddWithValue("@Phone", (object?)dto.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Addr", (object?)dto.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Dob", dto.Dob);
            cmd.Parameters.AddWithValue("@Gender", dto.Gender);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "DELETE FROM [user] WHERE Id = @Id";

            await connection.OpenAsync();
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
