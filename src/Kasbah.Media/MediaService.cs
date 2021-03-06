using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kasbah.Media.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Kasbah.Media
{
    public class MediaService
    {
        readonly IMediaProvider _mediaProvider;
        readonly IMediaStorageProvider _mediaStorageProvider;
        readonly IMemoryCache _cache;
        readonly ILogger _log;

        public MediaService(ILoggerFactory loggerFactory, IMediaProvider mediaProvider, IMediaStorageProvider mediaStorageProvider, IMemoryCache cache)
        {
            _log = loggerFactory.CreateLogger<MediaService>();
            _mediaProvider = mediaProvider;
            _mediaStorageProvider = mediaStorageProvider;
            _cache = cache;
        }

        public async Task<Guid> CreateMediaAsync(Stream source, string fileName, string contentType)
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

        public async Task<MediaItem> PutMediaAsync(MediaItem item)
            => await _mediaProvider.PutMediaAsync(item);

        public async Task<GetMediaResponse> GetMedia(GetMediaRequest request)
        {
            var item = await GetMediaItemAsync(request.Id);
            var stream = await GetMediaStreamAsync(request.Id);
            var responseStream = stream;

            if (!request.IsEmpty)
            {
                var cacheKey = $"media:{request.Hash}";
                var cached = _cache.Get<byte[]>(cacheKey);
                if (cached == null)
                {
                    var image = Image.Load(stream);
                    image.Mutate(i => i.Resize(new ResizeOptions
                    {
                        Size = CalculateSize(image.Width, image.Height, request.Width, request.Height)
                    }));

                    responseStream = new MemoryStream();
                    switch (item.ContentType)
                    {
                        case "media/gif":
                            image.SaveAsGif(responseStream);
                            break;
                        case "media/png":
                            image.SaveAsPng(responseStream);
                            break;
                        case "media/jpeg":
                        default:
                            image.SaveAsJpeg(responseStream);
                            break;
                    }

                    _cache.Set(cacheKey, (responseStream as MemoryStream).ToArray());

                    responseStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    responseStream = new MemoryStream(cached);
                }
            }

            if (responseStream != stream)
            {
                stream.Dispose();
            }

            return new GetMediaResponse
            {
                Stream = responseStream,
                Item = item
            };
        }

        Size CalculateSize(int sourceWidth, int sourceHeight, int? destWidth, int? destHeight)
        {
            if (destWidth.HasValue && destHeight.HasValue)
            {
                return new Size(destWidth.Value, destHeight.Value);
            }
            else if (destWidth.HasValue)
            {
                return new Size(destWidth.Value, sourceWidth / sourceHeight * destWidth.Value);
            }
            else if (destHeight.HasValue)
            {
                return new Size(sourceHeight / sourceWidth * destHeight.Value, destHeight.Value);
            }

            return new Size(sourceWidth, sourceHeight);
        }
    }
}
