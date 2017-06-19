using System.Threading.Tasks;
// using Kasbah.Logging;
// using Kasbah.Logging.Models;
using Kasbah.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Kasbah.Web.ContentDelivery.Jobs
{
    public class HeartbeatJob : IJob
    {
        public static ulong RequestsLatest { get; set; } = 0;

        public async Task Execute(IJobExecutionContext context)
        {
            var webApplication = Configure.Services.GetService<KasbahWebApplication>();
            // var loggingService = Configure.Services.GetService<LoggingService>();

            // await loggingService.HeartbeatAsync(new Heartbeat
            // {
            //     Instance = webApplication.Id,
            //     RequestsTotal = webApplication.RequestsTotal,
            //     RequestsLatest = RequestsLatest
            // });

            RequestsLatest = 0;

            await Task.Yield();
        }
    }
}
