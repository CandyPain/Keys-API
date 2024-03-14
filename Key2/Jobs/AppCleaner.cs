using Quartz;
using System.Net.Mail;
using System.Net;
using Key2.Services;
using Keys.Data;
using Key2.Models;
using Microsoft.EntityFrameworkCore;
using Keys.Models.Enums;

namespace Key2.Jobs
{
    public class AppCleaner : IAppCleaner
    {
        private readonly AppDbContext _context;
        public AppCleaner(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendEmailAsync()
        {
            DateTime utcNow = DateTime.UtcNow;
            IQueryable<Application> apps =  _context.apps.Include(k=>k.Key).ThenInclude(k=>k.CurrentUser).Include(u=>u.AppFromUser);
            foreach(var app in apps)
            {
                if(app.Date.Year == DateTime.Now.Year && app.Date.Month == DateTime.Now.Month && app.Date.Day == DateTime.Now.Day && app.IsConfirmation)
                {
                    DateTime shed = DateTime.UtcNow;
                    switch (app.Schedule)
                    {
                        case (ScheduleCell.First):
                            shed = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 1, 45, 0);
                            break;
                        case (ScheduleCell.Second):
                            shed = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 3, 35, 0);
                            break;
                        case (ScheduleCell.Third):
                            shed = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 5, 25, 0);
                            break;
                        case (ScheduleCell.Fourth):
                            shed = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 7, 45, 0);
                            break;
                        case (ScheduleCell.Fifth):
                            shed = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 9, 35, 0);
                            break;
                        case (ScheduleCell.Sixth):
                            shed = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 11, 25, 0);
                            break;
                        default:
                            shed = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 1, 45, 0);
                            break;
                    }
                    TimeSpan difference = utcNow - shed;
                    if (Math.Abs(difference.TotalMinutes) <= 2)
                    {
                        app.Key.CurrentUser = app.AppFromUser;
                        app.Key.CurrentUserId = app.AppFromUser.Id;
                    }
                }
            }
            var repeatedApps = await apps.Where(app => app.IsRepeat && app.Date < DateTime.Now).ToListAsync();
            foreach (var app in repeatedApps)
            {
                var newApp = new Application
                {
                    Id = Guid.NewGuid(),
                    Key = app.Key,
                    AppFromUser = app.AppFromUser,
                    Schedule = app.Schedule,
                    Date = app.Date.AddDays(7),
                    IsConfirmation = app.IsConfirmation,
                    IsRepeat = app.IsRepeat
                };
                _context.apps.Add(newApp);
            }
            IQueryable<Application> outdatedApps = apps.Where(app => app.Date < DateTime.Now);
            if (outdatedApps != null && outdatedApps.Count() > 0)
            {
                _context.apps.RemoveRange(outdatedApps);
            }

            await _context.SaveChangesAsync();
        }
    }
}
