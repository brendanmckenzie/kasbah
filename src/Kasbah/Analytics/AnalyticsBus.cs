using System.Collections.Generic;

namespace Kasbah.Analytics
{
    public class AnalyticsBus
    {
        readonly ICollection<IAnalyticsProcessor> _processors;
        readonly ICollection<string> _traits;
        public AnalyticsBus()
        {
            _processors = new List<IAnalyticsProcessor>();
        }

        public AnalyticsBus RegisterProcessor<TProcessor>()
            where TProcessor : IAnalyticsProcessor, new()
        {
            _processors.Add(new TProcessor());

            return this;
        }

        public AnalyticsBus RegisterTrait(string key)
        {
            _traits.Add(key);

            return this;
        }

        public IEnumerable<IAnalyticsProcessor> Processors => _processors;
        public IEnumerable<string> Traits => _traits;
    }
}
