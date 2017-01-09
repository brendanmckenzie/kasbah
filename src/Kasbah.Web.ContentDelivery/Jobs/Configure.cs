using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace Kasbah.Web.ContentDelivery.Jobs
{
    public static class Configure
    {
        // TODO: THIS WILL NOT LAST.
        public static IServiceProvider Services { get; private set; }

        public static void RegisterJobs(IServiceCollection services)
        {
            services.AddTransient<HeartbeatJob>();
        }

        public static async Task ConfigureJobsAsync(IServiceProvider services)
        {
            Services = services;

            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();

            await scheduler.Start();

            var job = JobBuilder.Create<HeartbeatJob>()
                .WithIdentity(nameof(HeartbeatJob))
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{nameof(HeartbeatJob)}_trigger")
                .StartNow()
                .WithSimpleSchedule(sched => sched.WithIntervalInMinutes(1))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}