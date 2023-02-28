using HotelListing.API.Data;
using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
        Task<AuthResponceDto> Login(LoginDto userDto);
        Task<string> CreateRefreshToken();
        Task<AuthResponceDto> VerifyRefreshToken(AuthResponceDto request);

        Task<string> GetUserId(string email);
        Task<bool> PromoteToAdmin(string userId);
    }
}
