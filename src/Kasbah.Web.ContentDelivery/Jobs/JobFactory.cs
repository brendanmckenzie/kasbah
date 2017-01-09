using System;
using Quartz;
using Quartz.Spi;

namespace Kasbah.Web.ContentDelivery.Jobs
{
    public class JobFactory : IJobFactory
    {
        readonly IServiceProvider _services;

        public JobFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                return (IJob)_services.GetService(bundle.JobDetail.JobType);
            }
            catch (Exception e)
            {
                throw new SchedulerException("Problem instantiating class", e);
            }
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
