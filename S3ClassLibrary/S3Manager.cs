using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3ClassLibrary
{
    public class S3Manager
    {
        public AmazonS3Client S3Client { get; set; }
        public string BucketName { get; set; }

        public S3Manager(string accessKey, string secretKey)
        {
            AmazonS3Config config = new AmazonS3Config()
            {
                ServiceURL = "http://s3.yandexcloud.net:80",
                UseHttp = true,
                ForcePathStyle = true,
                ProxyHost = "s3.yandexcloud.net",
                ProxyPort = 80
            };
            AWSCredentials creds = new BasicAWSCredentials(accessKey, secretKey);
            S3Client = new AmazonS3Client(creds, config);
            //ListBucketsResponse response = await s3Client.ListBucketsAsync();
            /*foreach (S3Bucket b in response.Buckets)
            {
                Console.WriteLine("{0}\t{1}", b.BucketName, b.CreationDate);
            }*/
        }

        public S3Manager(string accessKey, string secretKey, string bucketName) : this(accessKey, secretKey)
        {
            BucketName = bucketName;
        }

        public async Task<ListBucketsResponse> GetListOfBucketsAsync()
        {
            ListBucketsResponse response = await S3Client.ListBucketsAsync();
            return response;
        }
        public async Task PutObjectAsync(string key, string text)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = this.BucketName,
                Key = key,
                ContentBody = text
            };
            await S3Client.PutObjectAsync(putObjectRequest);
            Console.WriteLine("Положили объект");
        }
        
        public async Task PutObjectAsync(string key, string filePath, MetadataCollection metadataCollection)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = key,
                FilePath = filePath
            };
            await S3Client.PutObjectAsync(putObjectRequest);
            Console.WriteLine("Положили объект");
        }
        
        /// <summary>
        /// Не проверено
        /// </summary>
        /// <param name="key"></param>
        public async Task GetObjectAsync(string key)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = key,
            };
            await S3Client.GetObjectAsync(getObjectRequest);
        }
    }
}