using Dapper;
using HotelListing.API.Data;
using HotelListing.API.Provider.Contracts;
using HotelListing.API.Provider.Exceptions;
using HotelListing.API.Provider.Models;
using Microsoft.Data.SqlClient;

namespace HotelListing.API.Provider
{
    public class AuthProvider : IAuthProvider
    {
        private readonly string _connectionString;

        public AuthProvider(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new NullReferenceException("Connection String is empty");
            }

            _connectionString = connectionString;
        }

        public async Task<SqlConnection> ConnectDb()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                throw new SqlConnectionFailedException("Connection mssql failed.");
            }
            return connection;
        }

        public async Task<IEnumerable<AuthRole>> GetAuthRolesAsync()
        {
            var connection = await ConnectDb();

            string sqlCommand = "SELECT * FROM [dbo].[AspNetRoles]";
            var roles = await connection.QueryAsync<AuthRole>(sqlCommand);
            return roles;
        }

        public async Task<IEnumerable<UserAuthRole>> GetUserAuthRolesAsync()
        {
            var connection = await ConnectDb();

            string sqlCommand = "SELECT * FROM [dbo].[AspNetUserRoles]";
            var roles = await connection.QueryAsync<UserAuthRole>(sqlCommand);
            return roles;
        }

        public async Task<bool> IsUserExistsAsync(string userId)
        {
            var connection = await ConnectDb();

            string sqlCommand = $"SELECT Id FROM [dbo].[AspNetUsers] " +
                                $"WHERE Id = '{userId}'";
            var user = await connection.QueryAsync<UserAuthRole>(sqlCommand);
            return user.Count() == 1 ? true : false;
        }

        public async Task<bool> AddRoleToUserAsync(string userId, string roleId)
        {
            var connection = await ConnectDb();

            var roles = await GetAuthRolesAsync();
            var isRoleExists = roles.Where(x => x.Id == roleId).Count() > 0;

            if (!isRoleExists)
            {
                return false;
            }

            string sqlCommand = $"INSERT INTO [dbo].[AspNetUserRoles]" +
                                $"VALUES('{userId}', '{roleId}')";
            await connection.ExecuteAsync(sqlCommand);
            return true;
        }

        public async Task<string> GetUserIdAsync(string email)
        {
            var connection = await ConnectDb();

            string sqlCommand = "SELECT Id FROM [dbo].[AspNetUsers] " +
                               $"WHERE Email = '{email}'";
            string userId = await connection.QueryFirstAsync<string>(sqlCommand);
            return userId;
        }
    }
}
