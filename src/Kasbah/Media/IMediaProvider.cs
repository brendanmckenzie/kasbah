using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Media.Models;

namespace Kasbah.Media
{
    public interface IMediaProvider
    {
        Task CreateMediaAsync(Guid id, string fileName, string contentType);
        Task<IEnumerable<MediaItem>> ListMediaAsync();
        Task<MediaItem> GetMediaAsync(Guid id);
        Task DeleteMediaAsync(Guid id);
    }
}
