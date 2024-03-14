using Key2.Models;
using Keys.Models.Enums;

namespace Key2.Services
{
    public interface IDeanService
    {
        Task<TokenResponseModel> RegisterAsync(RegisterDeanModel RegisterModel);
        Task<TokenResponseModel> LoginAsync(LoginCredentialsModel model);
        Task<DeanProfileModel> GetDeanProfileAsync(Guid DeanId);
        Task AcceptChangeRole(Guid AppId, bool IsAccept);
        Task<List<AppChangeRole>> GetListAppChange(Guid userId);
        Task LogOutAsync(TokenBan token);
        Task<List<Key>> GetAllKeys(Guid userId);
        Task<List<Key>> GetAllFreeKeys(Guid userId);
        Task<List<Dean>> GetDeans();
        Task<List<MobileUser>> GetAssignedUsers(Guid userId);
        Task AssignedDeanWorker(Guid userId, Guid deanId, bool isAssign);
        Task<List<MobileUser>> GetDeanWorkers(Guid deanId);
        Task EditProfile(Guid id, Dean dean);
    }
}
