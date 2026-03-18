using ArtistManagementSystem.Server.Interfaces;
using ArtistManagementSystem.Server.Models;
using ArtistManagementSystem.Server.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ArtistManagementSystem.Server.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = "SELECT * FROM [user] WHERE Email = @Email";

            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();

            UserModel? user = null;

            if (await reader.ReadAsync())
            {
                user = new UserModel
                {
                    Id = (int)reader["Id"],
                    FirstName = reader["FirstName"].ToString()!,
                    LastName = reader["LastName"].ToString()!,
                    Email = reader["Email"].ToString()!,
                    Password = reader["Password"].ToString()!,
                    Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                    Dob = (DateTime)reader["Dob"],
                    Gender = Enum.Parse<Gender>(reader["Gender"].ToString()!, true),
                    Role = Enum.Parse<RoleType>(reader["Role"].ToString()!, true),
                    Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                    CreatedAt = (DateTime)reader["CreatedAt"]
                };
            }

            return user;
        }
        public async Task<bool> UserExistsAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            const string query = "SELECT COUNT(1) FROM [user] WHERE Email = @Email";

            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            var count = (int)await command.ExecuteScalarAsync()!;
            return count > 0;
        }
        public async Task<int> RegisterUserAsync(UserModel user, string roleName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                const string userSql = @"
                                        INSERT INTO [user] (FirstName, LastName, Email, Password, Phone, Address, Dob, Gender, Role, CreatedAt) 
                                        VALUES (@FN, @LN, @Email, @Pass, @Phone, @Addr, @Dob, @Gender, @Role, @Created);
                                        SELECT SCOPE_IDENTITY();";

                using var userCmd = new SqlCommand(userSql, connection);
                userCmd.Parameters.AddWithValue("@FN", user.FirstName);
                userCmd.Parameters.AddWithValue("@LN", user.LastName);
                userCmd.Parameters.AddWithValue("@Email", user.Email);
                userCmd.Parameters.AddWithValue("@Pass", user.Password);
                userCmd.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
                userCmd.Parameters.AddWithValue("@Addr", (object?)user.Address ?? DBNull.Value);
                userCmd.Parameters.AddWithValue("@Dob", user.Dob);
                userCmd.Parameters.AddWithValue("@Gender", user.Gender.ToString());
                userCmd.Parameters.AddWithValue("@Role", roleName);
                userCmd.Parameters.AddWithValue("@Created", user.CreatedAt);

                var result = await userCmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
