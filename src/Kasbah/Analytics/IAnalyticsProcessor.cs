using System.Threading.Tasks;
using Kasbah.Analytics.Models;

namespace Kasbah.Analytics
{
    public delegate void SetAttribute(string attribute, string value);
    public delegate void IncrementTrait(string trait, long weight);

    public interface IAnalyticsProcessor
    {
        Task HandleEvent(Profile profile, AnalyticsEvent ev, SetAttribute setAttribute, IncrementTrait incTrait);
    }
}
