
using Keys.Data;
using Quartz;

namespace Key2.Services
{
    public interface IFactory
    {
        IJob Create(AppDbContext context);
    }
}
