using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess;
using Kasbah.Media.Models;
using Microsoft.Extensions.Logging;

namespace Kasbah.Media
{
    public class MediaService
    {
        readonly IMediaProvider _mediaProvider;
        readonly IMediaStorageProvider _mediaStorageProvider;
        readonly ILogger _log;

        public MediaService(ILoggerFactory loggerFactory, IMediaProvider mediaProvider, IMediaStorageProvider mediaStorageProvider)
        {
            _log = loggerFactory.CreateLogger<MediaService>();
            _mediaProvider = mediaProvider;
            _mediaStorageProvider = mediaStorageProvider;
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(MediaService)}");

            await Task.Delay(0);
        }

        public async Task<Guid> PutMediaAsync(Stream source, string fileName, string contentType)
        {
            var id = await _mediaStorageProvider.StoreMediaAsync(source);

            await _mediaProvider.CreateMediaAsync(id, fileName, contentType);

            return id;
        }

        public async Task<IEnumerable<MediaItem>> ListMediaAsync()
            => await _mediaProvider.ListMediaAsync();

        public async Task<Stream> GetMediaStreamAsync(Guid id)
            => await _mediaStorageProvider.GetMediaAsync(id);

        public async Task<MediaItem> GetMediaItemAsync(Guid id)
            => await _mediaProvider.GetMediaAsync(id);

        public async Task DeleteMediaItemAsync(Guid id)
        {
            await Task.WhenAll(
                _mediaProvider.DeleteMediaAsync(id),
                _mediaStorageProvider.DeleteMediaAsync(id));
        }
    }
}
