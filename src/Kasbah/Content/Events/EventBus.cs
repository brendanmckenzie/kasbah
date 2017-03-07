using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content.Models;

namespace Kasbah.Content.Events
{
    public class EventBus
    {
        readonly IEnumerable<IOnContentPublished> _contentPublishedHandlers;

        public EventBus(IEnumerable<IOnContentPublished> contentPublishedHandlers)
        {
            _contentPublishedHandlers = contentPublishedHandlers;
        }

        public async Task TriggerContentPublished(Node node)
        {
            var tasks = _contentPublishedHandlers?.Select(ent => ent.ContentPublishedAsync(node));

            await Task.WhenAll(tasks);
        }
    }
}
