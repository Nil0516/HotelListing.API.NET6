﻿using HotelListing.API.Data;
using HotelListing.API.Core.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Core.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
        Task<AuthResponseDto> Login(LoginDto userDto);
        Task<string> CreateRefreshToken();
        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);

        Task<string> GetUserId(string email);
        Task<bool> PromoteToAdmin(string userId);
    }
}
