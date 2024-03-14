using Key2.Models;
using Key2.Services;
using Keys.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Key2.Controllers
{
    [Route("api/key")]
    public class KeyController : Controller
    {
        private readonly IKeyService _keyService;
        private readonly IBannedTokenService _banTokensService;

        public KeyController(IKeyService keyService, IBannedTokenService banTokensService)
        {
            _keyService = keyService;
            _banTokensService = banTokensService;
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateKey([FromBody]int Number,[FromBody] int building = 0)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _keyService.CreateKey(Number,userId,building);
                return StatusCode(200, res);
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
            catch (RankException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> DeleteKey([FromBody] Guid id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _keyService.DeleteKey(id, userId);
                return StatusCode(200, "Ok");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, "Not Found");
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (RankException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("handOver")]
        [HttpPut]
        public async Task<IActionResult> HandOver(Guid KeyId, Guid toUserId)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _keyService.HandOver(userId,KeyId, toUserId);
                return StatusCode(200, "Ok");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, "Not Found");
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (RankException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("listKeys")]
        [HttpGet]
        public async Task<IActionResult> ListKeys(DateTime date, ScheduleCell cell)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _keyService.ListKeys(date,cell);
                return StatusCode(200, res);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, "Not Found");
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (RankException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("schedule")]
        [HttpGet]
        public async Task<IActionResult> Schedule(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _keyService.SchedulePreviews(Id);
                return StatusCode(200, res);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, "Not Found");
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (RankException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("return")]
        [HttpPut]
        public async Task<IActionResult> Return(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _keyService.ReturnKey(userId, Id);
                return StatusCode(200, "Ok");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (RankException ex)
            {
                return StatusCode(403, ex.Message);
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
        [Route("allSchedule")]
        [HttpGet]
        public async Task<IActionResult> AllSchedule(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _keyService.AllSchedule(Id);
                return StatusCode(200, res);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, "Not Found");
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (RankException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [Route("allKeys")]
        [HttpGet]
        public async Task<IActionResult> AllKeys()
        {
            try
            {
                var res = await _keyService.AllKeys();
                return StatusCode(200, res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
}

        [Route("myKeys")]
        [HttpGet]
        public async Task<IActionResult> MyKeys(Guid Id)
        {
            try
            {
                var res = await _keyService.MyKeys(Id);
                return StatusCode(200, res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
