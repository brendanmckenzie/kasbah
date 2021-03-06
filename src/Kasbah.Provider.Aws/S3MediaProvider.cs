using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Kasbah.Media;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Kasbah.Provider.Aws
{
    public class S3MediaProvider : IMediaStorageProvider
    {
        readonly S3MediaProviderSettings _settings;
        readonly IMemoryCache _cache;

        public S3MediaProvider(S3MediaProviderSettings settings, IMemoryCache cache)
        {
            _settings = settings;
            _cache = cache;
        }

        public async Task<Stream> GetMediaAsync(Guid id)
        {
            var cacheKey = $"media:s3:{id}";
            var cached = _cache.Get<byte[]>(cacheKey);
            if (cached == null)
            {
                using (var client = GetClient())
                {
                    try
                    {
                        var ret = await client.GetObjectAsync(new GetObjectRequest
                        {
                            BucketName = _settings.BucketName,
                            Key = id.ToString()
                        });

                        var stream = new MemoryStream();
                        await ret.ResponseStream.CopyToAsync(stream);

                        _cache.Set(cacheKey, stream.ToArray());

                        stream.Seek(0, SeekOrigin.Begin);

                        return stream;
                    }
                    catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
                    {
                        return null;
                    }
                }
            }
            else
            {
                return new MemoryStream(cached);
            }
        }

        public async Task<Guid> StoreMediaAsync(Stream source)
        {
            using (var client = GetClient())
            {
                var id = Guid.NewGuid();
                await client.PutObjectAsync(new PutObjectRequest
                {
                    Key = id.ToString(),
                    BucketName = _settings.BucketName,
                    InputStream = source
                });

                return id;
            }
        }

        public async Task DeleteMediaAsync(Guid id)
        {
            using (var client = GetClient())
            {
                var ret = await client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = id.ToString()
                });
            }
        }

        AmazonS3Client GetClient()
            => new AmazonS3Client(
                new BasicAWSCredentials(_settings.AwsAccessKeyId, _settings.AwsSecretAccessKey),
                RegionEndpoint.GetBySystemName(_settings.Region));
    }
}
