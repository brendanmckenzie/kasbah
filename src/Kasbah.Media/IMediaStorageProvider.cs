using System;
using System.IO;
using System.Threading.Tasks;

namespace Kasbah.Media
{
    public interface IMediaStorageProvider
    {
        Task<Guid> StoreMediaAsync(Stream source);

        Task<Stream> GetMediaAsync(Guid id);

        Task DeleteMediaAsync(Guid id);
    }
}
