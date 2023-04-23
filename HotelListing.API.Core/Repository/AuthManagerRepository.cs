using AutoMapper;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Core.Models.Users;
using HotelListing.API.Core.Provider;
using HotelListing.API.Core.Provider.Contracts;
using HotelListing.API.Core.Provider.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Core.Repository
{
    public class AuthManagerRepository : IAuthManager
    {
        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";
        
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly UserManager<ApiUser> _userManager;
        private ApiUser _user;

        private readonly string ADMINISTRATOR = "Administrator";
        private readonly string USER = "User";
        private readonly IAuthProvider _authProvider;

        public AuthManagerRepository(IMapper mapper, IConfiguration configuration, IAuthProvider authProvider, UserManager<ApiUser> userManager, 
                                    ILogger<AuthManagerRepository> logger)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _authProvider = authProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = userDto.Email;

            var result = await _userManager.CreateAsync(_user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            _logger.LogInformation($"Looking for user with emal {loginDto.Email}");

            _user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (_user is null)
            {
                _logger.LogWarning($"User with email {loginDto.Email} was default.");
                return default;
            }

            bool isValidCredentials = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

            if (!isValidCredentials)
            {
                return default;
            }

            var token = await GenerateToken();
            return new AuthResponseDto { UserId = _user.Id, Token = token, RefreshToken = await CreateRefreshToken() };
        }

        public async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id)
            }.Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);
            return newRefreshToken;
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByEmailAsync(username);

            if (_user == null || _user.Id != request.UserId)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);

            if (isValidRefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }

        public async Task<bool> PromoteToAdmin(string userId)
        {
            if (!await _authProvider.IsUserExistsAsync(userId))
            {
                throw new UserNotFoundException("Failed to find user");
            }

            var roles = await _authProvider.GetAuthRolesAsync();
            var adminId = roles.Where(id => id.Name == ADMINISTRATOR).FirstOrDefault();

            return await _authProvider.AddRoleToUserAsync(userId, adminId.Id);
        }

        public async Task<string> GetUserId(string email)
        {
            // TODO: 若有要維護，應該要放在 MemberService 的層次。
            return await _authProvider.GetUserIdAsync(email);
        }
    }
}
