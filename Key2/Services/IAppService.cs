using Key2.Models;

namespace Key2.Services
{
    public interface IAppService
    {
        Task<Guid> CreateApp(CreateAppModel model, Guid userId);
        Task<List<Application>> ShowApps(Guid userId);
        Task ConfirmApp(Guid userId, Guid Id, bool isConfirm);
        Task<List<Application>> ShowAppsForKey(Guid keyId);
        Task DeleteApp(Guid userId, Guid Id);
    }
}
