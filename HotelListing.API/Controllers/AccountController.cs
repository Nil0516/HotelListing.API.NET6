﻿using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody]ApiUserDto apiUserDto)
        {
            _logger.LogInformation($"Registration Attemp for {apiUserDto.Email}");
            var errors = await _authManager.Register(apiUserDto);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authReponse = await _authManager.Login(loginDto);

            if (authReponse == null)
            {
                return Unauthorized();
            }

            return Ok(authReponse);
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("refreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            var authReponse = await _authManager.VerifyRefreshToken(request);

            if (authReponse == null)
            {
                return Unauthorized();
            }

            return Ok(authReponse);
        }

        // POST: api/Account/register
        [HttpGet]
        [Route("addAdminRoleToUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddAdminRoleToUser(string userId)
        {
            if (await _authManager.PromoteToAdmin(userId))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/Account/register
        [HttpGet]
        [Route("userId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserId(string email)
        {
            var userId = await _authManager.GetUserId(email);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("Failed to get user id.");
            } else
            {
                return Ok(userId);
            }
        }
    }
}
