using Key2.Models;

namespace Key2.Services
{
    public interface IBannedTokenService
    {
        void CheckAuthentication(TokenBan token);
    }
}
