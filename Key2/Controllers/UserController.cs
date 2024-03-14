using Key2.Models;
using Key2.Services;
using Key2.Services;
using Keys.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Claims;

namespace Key2.Controllers
{
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBannedTokenService _banTokensService;

        public UserController(IUserService userService, IBannedTokenService banTokensService)
        {
            _userService = userService;
            _banTokensService = banTokensService;
        }

        [Authorize]
        [Route("getRole")]
        [HttpGet]
        public async Task<IActionResult> GetRole()
        {
            try
            {
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _userService.GetRoleAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var res = await _userService.RegisterAsync(model);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

            [Route("login")]
            [HttpPost]
            async Task<IActionResult> Login([FromBody] LoginCredentialsModel LC)
            {
                try
                {
                    var result = await _userService.LoginAsync(LC);
                    return Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error");
                }
            }
            [Route("logout")]
            [HttpPost]
            async Task<IActionResult> LogOut()
            {
                try
                {
                    TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                    _banTokensService.CheckAuthentication(token);
                    await _userService.LogOutAsync(token);
                    return Ok("LogOut successfully");
                }
                catch (AuthenticationException ex)
                {
                    return StatusCode(401, "Autorization Error");
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }
                catch (Exception ex) { return StatusCode(500, "Internal server error"); }
            }

            [Route("profile")]
            [HttpGet]
            [Authorize]
            async Task<IActionResult> GetProfile()
            {
                try
                {
                    TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                    _banTokensService.CheckAuthentication(token);
                    Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var result = await _userService.GetProfileAsync(userId);
                    return Ok(result);
                }
                catch (DirectoryNotFoundException ex)
                {
                    return StatusCode(404, "Not Found");
                }
                catch (AuthenticationException ex)
                {
                    return StatusCode(401, "Autorization Error");
                }
                catch (Exception ex) { return StatusCode(500, "Internal server error"); }
            }

            [Route("profile")]
            [HttpPut]
            [Authorize]
            async Task<IActionResult> EditProfile([FromBody] RegisterModel userEditModel)
            {
                try
                {
                    if (userEditModel == null) { throw new DirectoryNotFoundException(); }
                    TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                    _banTokensService.CheckAuthentication(token);
                    Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    await _userService.EditProfileAsync(userEditModel, userId);
                    return StatusCode(200, "OK");
                }
                catch (DirectoryNotFoundException ex)
                {
                    return StatusCode(404, "Not Found");
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }
                catch (AuthenticationException ex)
                {
                    return StatusCode(401, "Autorization Error");
                }
                catch (Exception ex) { return StatusCode(500, "Internal server error"); }
            }

            [Route("createChangeRole")]
            [HttpPost]
            async Task<IActionResult> CreateChangeRole(Role desiredRole, Guid deanId)
            {
                try
                {
                    TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                    _banTokensService.CheckAuthentication(token);
                    Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    await _userService.CreateChangeRole(userId, desiredRole, deanId);
                    return StatusCode(200, "OK");
                }
                catch (DirectoryNotFoundException ex)
                {
                    return StatusCode(404, "Not Found");
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }
                catch (AuthenticationException ex)
                {
                    return StatusCode(401, "Autorization Error");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error");
                }
            }

            [Route("createQR")]
            [HttpPost]
            async Task<IActionResult> CreateQR(Guid keyId, string pass)
            {
                try
                {
                    TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                    _banTokensService.CheckAuthentication(token);
                    Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    await _userService.CreateQR(userId, keyId, pass);
                    return StatusCode(200, "OK");
                }
                catch (DirectoryNotFoundException ex)
                {
                    return StatusCode(404, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }
                catch (RankException ex)
                {
                    return StatusCode(403, ex.Message);
                }
                catch (AuthenticationException ex)
                {
                    return StatusCode(401, "Autorization Error");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error");
                }
            }

            [Route("readQR")]
            [HttpPut]
            async Task<IActionResult> ReadQR(Guid keyId, string pass)
            {
                try
                {
                    TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                    _banTokensService.CheckAuthentication(token);
                    Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    await _userService.ReadQR(userId, keyId, pass);
                    return StatusCode(200, "OK");
                }
                catch (DirectoryNotFoundException ex)
                {
                    return StatusCode(404, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }
                catch (RankException ex)
                {
                    return StatusCode(403, ex.Message);
                }
                catch (AuthenticationException ex)
                {
                    return StatusCode(401, "Autorization Error");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error");
                }
            }

            [Route("deleteQr")]
            [HttpDelete]
            async Task<IActionResult> DeleteQR(Guid keyId)
            {
                try
                {
                    TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                    _banTokensService.CheckAuthentication(token);
                    Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    await _userService.DeleteQr(keyId);
                    return StatusCode(200, "OK");
                }
                catch (DirectoryNotFoundException ex)
                {
                    return StatusCode(404, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return StatusCode(400, ex.Message);
                }
                catch (RankException ex)
                {
                    return StatusCode(403, ex.Message);
                }
                catch (AuthenticationException ex)
                {
                    return StatusCode(401, "Autorization Error");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error");
                }
            }
        }
    }
}
