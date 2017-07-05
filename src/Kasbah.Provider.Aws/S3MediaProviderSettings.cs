namespace Kasbah.Provider.Aws
{
    public class S3MediaProviderSettings
    {
        public string BucketName { get; set; }

        public string Region { get; set; }

        public string AwsAccessKeyId { get; set; }

        public string AwsSecretAccessKey { get; set; }
    }
}
