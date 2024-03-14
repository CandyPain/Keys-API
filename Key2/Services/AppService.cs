using Key2.Models;
using Keys.Data;
using Microsoft.EntityFrameworkCore;

namespace Key2.Services
{
    public class AppService:IAppService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        public AppService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }

        async Task IAppService.ConfirmApp(Guid userId, Guid Id, bool isConfirm)
        {
            var dean = await _context.deans.FirstOrDefaultAsync(d => d.Id == userId);
            if(dean == null)
            {
                throw new RankException("Forbidden");
            }
            var app = await _context.apps.FirstOrDefaultAsync(a => a.Id == Id);
            if(app == null)
            {
                throw new ArgumentException("Bad App Id");
            }
            if(isConfirm)
            {
                app.IsConfirmation = true;
            }
            else
            {
                _context.apps.Remove(app);
            }
            _context.SaveChanges();
        }

        async Task<Guid> IAppService.CreateApp(CreateAppModel model, Guid userId)
        {
            var key = await _context.keys.FirstOrDefaultAsync(k=>k.Id == model.KeyId);
            Dean dean = null;
            var user = await _context.mobileUsers.FirstOrDefaultAsync(u=>u.Id == userId);
            if (user == null)
            {
                dean = await _context.deans.FirstOrDefaultAsync(d=>d.Id == userId);
                if (dean == null)
                {
                    throw new DirectoryNotFoundException("user not found");
                }
            }
            var CheckApp = await _context.apps.FirstOrDefaultAsync(a => a.Date.Year == model.Date.Year && a.Date.Month == model.Date.Month && a.Date.Day == model.Date.Day && a.Schedule == model.Schedule && a.IsConfirmation == true);
            if(CheckApp != null) {
                throw new ArgumentException("Заявка на это время уже создана.");
            }
            if (dean != null || (user.IsDeanWorker == true && user.DeanId == key.DeanId))
            {
                Application app = new Application
                {
                    Id = Guid.NewGuid(),
                    AppFromUser = dean,
                    Date = model.Date,
                    IsConfirmation = true,
                    Key = key,
                    Schedule = model.Schedule,
                    IsRepeat = model.isRepeat
                };
                _context.apps.Add(app);
                _context.SaveChanges();
                return app.Id;
            }
            else
            {
                Application app = new Application
                {
                    Id = Guid.NewGuid(),
                    AppFromUser = user,
                    Date = model.Date,
                    IsConfirmation = false,
                    Key = key,
                    Schedule = model.Schedule,
                };
                _context.apps.Add(app);
                _context.SaveChanges();
                return app.Id;
            }
        }

        async Task IAppService.DeleteApp(Guid userId, Guid Id)
        {
            var dean = await _context.deans.SingleOrDefaultAsync(d=>d.Id == userId && d.IsActive == true);
            if(dean == null)
            {
                throw new RankException("403 Forbidden");
            }
            var app = await _context.apps.SingleOrDefaultAsync(a => a.Id == Id);
            if(app == null)
            {
                throw new DirectoryNotFoundException("App not Found");
            }
            _context.apps.Remove(app);
            _context.SaveChanges();
        }

        async Task<List<Application>> IAppService.ShowApps(Guid userId)
        {
            List<Application> apps = await _context.apps.Include(k=>k.Key).Include(u=>u.AppFromUser).Where(a => a.IsConfirmation == false && a.Key.DeanId==userId).ToListAsync();
            return apps;
        }

        async Task<List<Application>> IAppService.ShowAppsForKey(Guid keyId)
        {
            List<Application> apps = await _context.apps.Include(k => k.Key).Where(a => a.Key.Id == keyId && a.IsConfirmation == true).ToListAsync();
            return apps;
        }
    }
}
