using System;
using System.IO;
using System.Threading.Tasks;

namespace Kasbah.Media
{
    public class LocalStorageMediaProvider : IMediaStorageProvider
    {
        readonly LocalStorageMediaProviderSettings _settings;

        public LocalStorageMediaProvider(LocalStorageMediaProviderSettings settings)
        {
            _settings = settings;

            if (!Directory.Exists(settings.ContentRoot))
            {
                Directory.CreateDirectory(settings.ContentRoot);
            }
        }

        public async Task<Stream> GetMediaAsync(Guid id)
        {
            return await Task.FromResult(File.OpenRead(GetPath(id)) as Stream);
        }

        public async Task<Guid> StoreMediaAsync(Stream stream)
        {
            var id = Guid.NewGuid();

            using (var dest = File.OpenWrite(GetPath(id)))
            {
                await stream.CopyToAsync(dest);
            }

            return id;
        }

        public async Task DeleteMediaAsync(Guid id)
        {
            File.Delete(GetPath(id));

            await Task.Delay(0);
        }

        string GetPath(Guid id)
            => Path.Combine(_settings.ContentRoot, id.ToString());
    }
}
