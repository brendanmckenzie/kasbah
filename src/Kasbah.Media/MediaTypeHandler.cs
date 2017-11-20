using System;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Media.Models;
using Microsoft.Extensions.Logging;

namespace Kasbah.Media
{
    public class MediaTypeHandler : ITypeHandler
    {
        readonly IMediaProvider _mediaProvider;
        readonly ILogger _log;

        public MediaTypeHandler(IMediaProvider mediaProvider, ILoggerFactory loggerFactory)
        {
            _mediaProvider = mediaProvider;
            _log = loggerFactory.CreateLogger<MediaTypeHandler>();
        }

        public string DefaultEditor => "mediaPicker";

        public bool CanConvert(Type type)
            => typeof(MediaItem).GetTypeInfo().IsAssignableFrom(type);

        public async Task<object> ConvertAsync(object source)
            => await MapLinkedMediaAsync(source);

        async Task<object> MapLinkedMediaAsync(object source)
        {
            Guid id;
            if (Guid.TryParse((string)source, out id))
            {
                try
                {
                    return await _mediaProvider.GetMediaAsync(id);
                }
                catch
                {
                    _log.LogDebug($"Failed to map linked media {id}");
                    return null;
                }
            }

            return null;
        }
    }
}
