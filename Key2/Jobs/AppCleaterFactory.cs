using Key2.Services;
using Keys.Data;
using Quartz;

namespace Key2.Jobs
{
    public class AppCleaterFactory : IFactory
    {
        IJob IFactory.Create(AppDbContext context)
        {
            return null;
            //return new EmailSender(context);
        }
    }
}
