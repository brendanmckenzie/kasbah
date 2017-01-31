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
        const string IndexName = "media";
        readonly IDataAccessProvider _dataAccessProvider;
        readonly IMediaStorageProvider _mediaStorageProvider;
        readonly ILogger _log;

        public MediaService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider, IMediaStorageProvider mediaStorageProvider)
        {
            _log = loggerFactory.CreateLogger<MediaService>();
            _dataAccessProvider = dataAccessProvider;
            _mediaStorageProvider = mediaStorageProvider;
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(MediaService)}");
            await _dataAccessProvider.EnsureIndexExists(IndexName);
        }

        public async Task<Guid> PutMediaAsync(Stream source, string fileName, string contentType)
        {
            var id = await _mediaStorageProvider.StoreMediaAsync(source);

            var mediaItem = new MediaItem
            {
                Id = id,
                FileName = fileName,
                ContentType = contentType
            };

            await _dataAccessProvider.PutEntryAsync(IndexName, id, mediaItem);

            return id;
        }

        public async Task<IEnumerable<MediaItem>> ListMediaAsync()
        {
            var entries = await _dataAccessProvider.QueryEntriesAsync<MediaItem>(IndexName, take: 1024);

            return entries.Select(ent => ent.Source);
        }

        public async Task<Stream> GetMediaStreamAsync(Guid id)
            => await _mediaStorageProvider.GetMediaAsync(id);

        public async Task<MediaItem> GetMediaItemAsync(Guid id)
            => (await _dataAccessProvider.GetEntryAsync<MediaItem>(IndexName, id)).Source;
    }
}
