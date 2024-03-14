using Key2.Models;
using Key2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Key2.Controllers
{
    [Route("api/app")]
    public class AppController : Controller
    {
        private readonly IAppService _appService;
        private readonly IBannedTokenService _banTokensService;

        public AppController(IAppService appService, IBannedTokenService banTokensService)
        {
            _appService = appService;
            _banTokensService = banTokensService;
        }


        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateApp(CreateAppModel model)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _appService.CreateApp(model,userId);
                return StatusCode(200, res);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
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

        [Route("show")]
        [HttpGet]
        public async Task<IActionResult> ShowApps()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _appService.ShowApps(userId);
                return StatusCode(200, res);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
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



        [Route("confirm")]
        [HttpPut]
        public async Task<IActionResult> ConfirmApp(Guid id, bool IsConfirm)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _appService.ConfirmApp(userId,id,IsConfirm);
                return StatusCode(200, "Ok");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
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


        [Route("key/show")] //Показываем одобренные заявки по ключу
        [HttpGet]
        public async Task<IActionResult> ShowAppsForKey(Guid keyId)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _appService.ShowAppsForKey(keyId);
                return StatusCode(200, res);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
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

        [Route("delete")] //Показываем одобренные заявки по ключу
        [HttpDelete]
        public async Task<IActionResult> DeleteApp(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _appService.DeleteApp(userId,Id);
                return StatusCode(200, "Ok");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
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
    }
}
