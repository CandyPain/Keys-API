
using Key2.Models;
using Key2.Services;
using Keys.Data;
using System.Security.Authentication;

namespace Key2.Services
{
    public class BannedTokensService : IBannedTokenService
    {
        private readonly AppDbContext _context;
        public BannedTokensService(AppDbContext context)
        {
            _context = context;
        }

        public void CheckAuthentication(TokenBan token)
        {
            var banToken = _context.TokensBan.FirstOrDefault(t => t.BannedToken == token.BannedToken);
            if (banToken != null)
            {
                throw new AuthenticationException();
            }
        }
    }
}
