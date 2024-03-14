using Quartz.Impl;
using Quartz.Spi;
using Quartz;

namespace Key2.Jobs
{
    public static class DataScheduler
    {

        public static async void Start(IServiceProvider serviceProvider)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = serviceProvider.GetService<JobFactory>();
            await scheduler.Start();

            IJobDetail jobDetail1 = JobBuilder.Create<DataJob>().Build();
            IJobDetail jobDetail2 = JobBuilder.Create<DataJob>().Build();
            IJobDetail jobDetail3 = JobBuilder.Create<DataJob>().Build();
            // Запуск в 8:45 14:45, каждый день
            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity("MailingTrigger1", "default")
                .WithCronSchedule("0 45 8,14 * * ?")
                .Build();

            // Запуск в 10:35 и 16:35 каждый день
            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("MailingTrigger2", "default")
                .WithCronSchedule("0 35 10,16 * * ?")
                .Build();

            // Запуск в 12:25 и 18:25 каждый день
            ITrigger trigger3 = TriggerBuilder.Create()
                .WithIdentity("MailingTrigger3", "default")
                .WithCronSchedule("0 25 12,18 * * ?")
                .Build();

            await scheduler.ScheduleJob(jobDetail1, trigger1);
            await scheduler.ScheduleJob(jobDetail2, trigger2);
            await scheduler.ScheduleJob(jobDetail3, trigger3);
            var executingJobs = await scheduler.GetCurrentlyExecutingJobs();
            Console.WriteLine(executingJobs.Count);
        }
    }
}
