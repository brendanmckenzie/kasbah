using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Kasbah.Media.S3
{
    public class S3MediaProvider : IMediaStorageProvider
    {
        readonly S3MediaProviderSettings _settings;

        public S3MediaProvider(S3MediaProviderSettings settings)
        {
            _settings = settings;
        }

        public async Task<Stream> GetMediaAsync(Guid id)
        {
            using (var client = GetClient())
            {
                var ret = await client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = id.ToString()
                });

                return ret.ResponseStream;
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
