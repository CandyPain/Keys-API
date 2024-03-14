using Key2.Models;
using Key2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Key2.Controllers
{
    [Route("api/dean")]
    public class DeanController : ControllerBase
    {
        private readonly IDeanService _deanService;
        private readonly IBannedTokenService _banTokensService;

        public DeanController(IDeanService deanService, IBannedTokenService banTokensService)
        {
            _deanService = deanService;
            _banTokensService = banTokensService;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDeanModel model)
        {
            var res = await _deanService.RegisterAsync(model);
            return Ok(res);
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsModel LC)
        {
            try
            {
                var result = await _deanService.LoginAsync(LC);
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
        public async Task<IActionResult> LogOut()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                await _deanService.LogOutAsync(token);
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
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _deanService.GetDeanProfileAsync(userId);
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

        [Route("editProfile")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditProfile(Dean dean)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _deanService.EditProfile(userId, dean);
                return Ok();
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

        [Route("acceptChangeRole")]
        [HttpPost]
        public async Task<IActionResult> AcceptChangeRole(Guid AppId, bool IsAccept)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                await _deanService.AcceptChangeRole(AppId, IsAccept);
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


        [Route("getListChangeRole")]
        [HttpGet]
        public async Task<IActionResult> GetListChangeRole()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _deanService.GetListAppChange(userId);
                return StatusCode(200, res);
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

        [Route("allKeys")]
        [HttpGet]
        public async Task<IActionResult> GetAllKeys()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _deanService.GetAllKeys(userId);
                return StatusCode(200, res);
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

        [Route("allFreeKeys")]
        [HttpGet]
        public async Task<IActionResult> GetAllFreeKeys()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _deanService.GetAllFreeKeys(userId);
                return StatusCode(200, res);
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
        [Route("GetDeans")]
        [HttpGet]
        public async Task<IActionResult> GetDeans()
        {
            var res = await _deanService.GetDeans();
            return StatusCode(200, res);
        }

        [Route("GetAssignedUsers")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAssignedUsers()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await _deanService.GetAssignedUsers(userId);
            return StatusCode(200, res);
        }

        [Route("AssignDeanWorker")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetAssignedUsers(Guid userId, bool isAssign)
        {
            Guid deanId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _deanService.AssignedDeanWorker(userId, deanId, isAssign);
            return StatusCode(200);
        }

        [Route("GetDeanWorkers")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDeanWorkers()
        {
            Guid deanId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await _deanService.GetDeanWorkers(deanId);
            return StatusCode(200, res);
        }
    }
}
