using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TesteADLogin.Model;
using TesteADLogin.Services;
using TesteADLogin.Data;
using Microsoft.Extensions.Logging;

namespace TesteADLogin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AzureAdAuthService _authService;
        private readonly UserDatabaseService _userDbService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AzureAdAuthService authService,
            UserDatabaseService userDbService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userDbService = userDbService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            try
            {
                var userInfo = await _authService.AuthenticateUserAsync(request.Username, request.Password);

                if (userInfo == null)
                    return Unauthorized();

                var dbUser = await _userDbService.GetUserByAzureIdAsync(userInfo.Id);

                if (dbUser == null)
                {
                    _logger.LogWarning("User not found in database: {Email}", userInfo.Email);
                    return NotFound("User not found in local database");
                }

                if (!dbUser.IsActive)
                {
                    return BadRequest("User account is inactive");
                }

                var completeUserInfo = new
                {
                    userInfo.Id,
                    userInfo.DisplayName,
                    userInfo.Email,
                    dbUser.Department,
                    dbUser.Role,
                    dbUser.IsActive,
                    LoginTimestamp = DateTime.UtcNow
                };

                return Ok(completeUserInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process for user: {Username}", request.Username);
                return StatusCode(500, "An error occurred during the login process");
            }
        }
    }


}
}
