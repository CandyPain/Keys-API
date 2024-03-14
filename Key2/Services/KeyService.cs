using Key2.Models;
using Keys.Data;
using Keys.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Key2.Services
{
    public class KeyService : IKeyService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        public KeyService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }
        async Task<Guid> IKeyService.CreateKey(int Number, Guid userId, int building = 0)
        {
            var dean = await _context.deans.FirstOrDefaultAsync(d => d.Id == userId);
            if(dean == null)
            {
                throw new RankException("403 Forbidden");
            }
            Key k = new Key();
            k.Number = Number;
            k.Id = Guid.NewGuid();
            k.CurrentUser = null;
            k.CurrentUserId = null;
            k.DeanId = dean.Id;
            k.Building = building;
            _context.keys.Add(k);
            _context.SaveChanges();
            return k.Id;
        }

        async Task IKeyService.DeleteKey(Guid Id, Guid userId)
        {
            var dean = await _context.deans.FirstOrDefaultAsync(d => d.Id == userId);
            if (dean == null)
            {
                throw new RankException("403 Forbidden");
            }
            Key k = await _context.keys.SingleOrDefaultAsync(k => k.Id == Id);
            if(k==null)
            {
                throw new DirectoryNotFoundException("Key not Found");
            }
            _context.keys.Remove(k);
            _context.SaveChanges();
        }

        async Task IKeyService.HandOver(Guid userId, Guid keyId, Guid toUserId)
        {
            var user = await _context.mobileUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new DirectoryNotFoundException("user not found"); 
            }
            var key = await _context.keys.Include(u=>u.CurrentUser).SingleOrDefaultAsync(k=>k.Id == keyId);
            if (key == null)
            {
                throw new DirectoryNotFoundException("KeyNotFound");
            }
            if(key.CurrentUser.Id != userId)
            {
                throw new RankException("Forbidden");
            }
            var newUser = await _context.mobileUsers.FirstOrDefaultAsync(u => u.Id == toUserId);
            if(newUser == null)
            {
                throw new DirectoryNotFoundException("Second user not found");
            }
            key.CurrentUser = newUser;
            _context.SaveChanges();
        }

        async Task<List<Key>> IKeyService.ListKeys(DateTime date, ScheduleCell cell)
        {
            IQueryable<Key> keys = _context.keys;
            IQueryable<Application> apps = _context.apps.Include(k=>k.Key);
            List<Key> lstKey = await keys.Where(k => apps.All(a => a.IsConfirmation == false || a.Key.Id != k.Id || a.Date.Year != date.Year || a.Date.Month != date.Month || a.Date.Day != date.Day || a.Schedule != cell)).ToListAsync();
            return lstKey;
            
        }

        async Task IKeyService.ReturnKey(Guid userId, Guid keyId)
        {
            var key = await _context.keys.FirstOrDefaultAsync(k=>k.Id == keyId);
            if(key == null)
            {
                throw new DirectoryNotFoundException("KeyNotFound");
            }
            if(key.CurrentUserId != userId)
            {
                throw new RankException("Forbidden");
            }
            key.CurrentUser = null;
            key.CurrentUserId = null;
            await _context.SaveChangesAsync();
        }

        async Task<List<KeySchedulePreview>> IKeyService.SchedulePreviews(Guid Id)
        {
            List<KeySchedulePreview> lst = await _context.apps.Include(a => a.Key).Where(a => a.Key.Id == Id && a.IsConfirmation == true && a.Date <= DateTime.Now.AddDays(7)).Select(item => new KeySchedulePreview { Date = item.Date, Sheduller = item.Schedule }).ToListAsync();
            return lst;
        }

        async Task<List<KeySchedulePreview>> IKeyService.AllSchedule(Guid Id)
        {
            List<KeySchedulePreview> lst = await _context.apps.Include(a => a.Key).Where(a => a.Key.Id == Id && a.IsConfirmation == true).Select(item => new KeySchedulePreview { Date = item.Date, Sheduller = item.Schedule, UserId = item.AppFromUser.Id, UserName = item.AppFromUser.Name }).ToListAsync();
            return lst;
        }

        async Task<List<Key>> IKeyService.AllKeys()
        {
            List<Key> lst = await _context.keys.ToListAsync();
            return lst;
        }

        async Task<List<Key>> IKeyService.MyKeys(Guid Id)
        {
            List<Key> lst = await _context.keys.Where(k => k.CurrentUserId == Id).ToListAsync();
            return lst;
        }
    }
}
