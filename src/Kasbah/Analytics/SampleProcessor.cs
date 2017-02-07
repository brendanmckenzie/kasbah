using System.Threading.Tasks;
using Kasbah.Analytics.Models;

namespace Kasbah.Analytics
{
    public sealed class SampleProcessor : IAnalyticsProcessor
    {
        public async Task HandleEvent(Profile profile, AnalyticsEvent ev, SetAttribute setAttribute, IncrementTrait incTrait)
        {
            switch (ev.Type)
            {
                case "set_name":
                    setAttribute.Invoke("name", ev.Data.SafeGet<string, string>("name"));
                    setAttribute.Invoke("first_name", ev.Data.SafeGet<string, string>("first_name"));
                    setAttribute.Invoke("last_name", ev.Data.SafeGet<string, string>("last_name"));
                    break;
            }

            await Task.Delay(0);
        }
    }
}
