using Key2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Key2.Controllers
{
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IBannedTokenService _banTokensService;

        public AdminController(IAdminService adminService, IBannedTokenService banTokensService)
        {
            _adminService = adminService;
            _banTokensService = banTokensService;
        }
    }
}
