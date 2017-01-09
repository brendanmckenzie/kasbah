using System.Threading.Tasks;
using Kasbah.Logging;
using Kasbah.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Kasbah.Web.ContentDelivery.Jobs
{
    public class HeartbeatJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var webApplication = Configure.Services.GetService<KasbahWebApplication>();
            var loggingService = Configure.Services.GetService<LoggingService>();

            await loggingService.HeartbeatAsync(webApplication.Id);
        }
    }
}