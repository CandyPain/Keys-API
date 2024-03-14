using Key2.Models;
using Keys.Models.Enums;
using System.Threading.Tasks;

namespace Key2.Services
{
    public interface IKeyService
    {
        Task<Guid> CreateKey(int Number, Guid userId, int building = 0);
        Task DeleteKey(Guid Id, Guid userId);
        Task HandOver(Guid userId, Guid keyId, Guid toUserId);
        Task<List<Key>> ListKeys(DateTime date,ScheduleCell cell);

        Task<List<KeySchedulePreview>> SchedulePreviews(Guid Id);
        Task<List<KeySchedulePreview>> AllSchedule(Guid Id);
        Task ReturnKey(Guid userId, Guid keyId);
        Task<List<Key>> AllKeys();

        Task<List<Key>> MyKeys(Guid Id);
    }
}
