﻿namespace HotelListing.API.Models.Users
{
    public class AuthResponceDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}