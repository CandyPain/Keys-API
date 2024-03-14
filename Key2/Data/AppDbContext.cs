using Key2.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Keys.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<MobileUser> mobileUsers { get; set; }
        public DbSet<Key> keys { get; set; }
        public DbSet<Application> apps { get; set; }
        public DbSet<Administrator> administrators { get; set; }
        public DbSet<TokenBan> TokensBan { get; set; }
        public DbSet<Dean> deans { get; set; }
        public DbSet<AppChangeRole> appChangeRoles { get; set; }
        public DbSet<QRPass> QRPass { get; set; }
    }
}