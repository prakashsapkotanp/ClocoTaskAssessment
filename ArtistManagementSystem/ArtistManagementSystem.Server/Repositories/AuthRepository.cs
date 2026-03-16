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

            // Using [UserRoles] (plural) and RoleId (PK) as defined in your migration
            string query = @"
        SELECT u.*, r.RoleName, r.Description
        FROM [user] u
        LEFT JOIN [UserRoles] ur ON u.Id = ur.UserId
        LEFT JOIN [role] r ON ur.RoleId = r.RoleId
        WHERE u.Email = @Email";

            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();

            UserModel? user = null;

            while (await reader.ReadAsync())
            {
                // Initialize the user object on the first row returned
                if (user == null)
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
                        // Convert string from DB to Gender Enum
                        Gender = Enum.Parse<Gender>(reader["Gender"].ToString()!, true),
                        Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        UserRoles = new List<UserRoleModel>()
                    };
                }

                // Add role to the list if it exists
                if (reader["RoleName"] != DBNull.Value)
                {
                    // Convert string from DB (e.g., "artist") to your RoleType Enum
                    var roleEnum = Enum.Parse<RoleType>(reader["RoleName"].ToString()!, true);

                    user.UserRoles.Add(new UserRoleModel
                    {
                        Role = new RoleModel
                        {
                            RoleName = roleEnum
                        }
                    });
                }
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
        public async Task<bool> RegisterUserAsync(UserModel user, string roleName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // fetch role by rolename.
                const string getRoleSql = "SELECT RoleId FROM [role] WHERE RoleName = @RoleName";
                int roleId;

                using (var roleLookupCmd = new SqlCommand(getRoleSql, connection, transaction))
                {
                    roleLookupCmd.Parameters.AddWithValue("@RoleName", roleName);
                    var roleResult = await roleLookupCmd.ExecuteScalarAsync();

                    if (roleResult == null || roleResult == DBNull.Value)
                    {
                        throw new Exception($"Invalid role: The role '{roleName}' was not found in the database.");
                    }

                    roleId = Convert.ToInt32(roleResult);
                }

                const string userSql = @"
                                        INSERT INTO [user] (FirstName, LastName, Email, Password, Dob, Gender,CreatedAt) 
                                        VALUES (@FN, @LN, @Email, @Pass, @Dob, @Gender, @Created);
                                        SELECT SCOPE_IDENTITY();";

                using var userCmd = new SqlCommand(userSql, connection, transaction);
                userCmd.Parameters.AddWithValue("@FN", user.FirstName);
                userCmd.Parameters.AddWithValue("@LN", user.LastName);
                userCmd.Parameters.AddWithValue("@Email", user.Email);
                userCmd.Parameters.AddWithValue("@Pass", user.Password);
                userCmd.Parameters.AddWithValue("@Dob", user.Dob);
                userCmd.Parameters.AddWithValue("@Gender", user.Gender.ToString());
                userCmd.Parameters.AddWithValue("@Created", user.CreatedAt);


                //var userId = (int)await userCmd.ExecuteScalarAsync()!;
                var result = await userCmd.ExecuteScalarAsync();
                int userId = Convert.ToInt32(result);

                const string roleSql = "INSERT INTO [UserRoles] (UserId, RoleId) VALUES (@UserId, @RoleId)";
                using var roleCmd = new SqlCommand(roleSql, connection, transaction);
                roleCmd.Parameters.AddWithValue("@UserId", userId);
                roleCmd.Parameters.AddWithValue("@RoleId", roleId);

                await roleCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                //return false;
                throw e;

            }
        }
    }
}
