using HotelListing.API.Data;
using HotelListing.API.Provider.Models;
using Microsoft.Data.SqlClient;

namespace HotelListing.API.Provider.Contracts
{
    public interface IAuthProvider
    {
        public Task<SqlConnection> ConnectDb();
        public Task<bool> IsUserExistsAsync(string userId);
        public Task<string> GetUserIdAsync(string email);
        public Task<IEnumerable<AuthRole>> GetAuthRolesAsync();
        public Task<IEnumerable<UserAuthRole>> GetUserAuthRolesAsync();
        public Task<bool> AddRoleToUserAsync(string userId, string roleId);
    }
}
