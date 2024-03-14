using Key2.Models;
using Keys.Models.Enums;

namespace Key2.Services
{
    public interface IUserService
    {
        Task<TokenResponseModel> RegisterAsync(RegisterModel RegisterModel);
        Task<TokenResponseModel> LoginAsync(LoginCredentialsModel model);
        Task<UserProfileModel> GetProfileAsync(Guid userId);
        Task EditProfileAsync(RegisterModel EditModel, Guid userId);
        Task CreateChangeRole(Guid userId, Role desiredRole, Guid deanId);
        Task LogOutAsync(TokenBan token);
        Task CreateQR(Guid userId,Guid keyId, string pass);
        Task ReadQR(Guid userId, Guid keyId, string pass);
        Task DeleteQr(Guid keyid);
        Task<RoleResponseModel> GetRoleAsync(Guid userId);
    }
}
