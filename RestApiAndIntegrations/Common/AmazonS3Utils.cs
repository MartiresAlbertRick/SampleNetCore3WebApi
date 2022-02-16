using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Common
{
    public static class AmazonS3Utils
    {
        const string DEFAULT_REGION = "ap-southeast-2";

        public static IAmazonS3 CreateClient(string awsAccessKeyId, string awsSecretAccessKey, string regionEndpoint = DEFAULT_REGION)
        {
            return new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, GetRegionEndpointBySystemName(regionEndpoint));
        }

        public static async Task UploadFileAsync(IAmazonS3 s3Client, Stream stream, string s3BucketName, string fileName)
        {
            using var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.UploadAsync(stream, s3BucketName, fileName).ConfigureAwait(false);
        }

        public static async Task UploadFileAsync(IAmazonS3 s3Client, string filePath, string s3BucketName, string fileName)
        {
            using var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.UploadAsync(filePath, s3BucketName, fileName).ConfigureAwait(false);
        }

        public static async Task DownloadFileAsync(IAmazonS3 s3Client, string filePath, string s3BucketName, string fileName)
        {
            using var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.DownloadAsync(filePath, s3BucketName, fileName).ConfigureAwait(false);
        }

        public static async Task<Stream> OpenStreamAsync(IAmazonS3 s3Client, string s3BucketName, string fileName)
        {
            using var fileTransferUtility = new TransferUtility(s3Client);
            return await fileTransferUtility.OpenStreamAsync(s3BucketName, fileName).ConfigureAwait(false);
        }

        private static RegionEndpoint GetRegionEndpointBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
            {
                throw new ArgumentException("Region systemName must be specified", nameof(systemName));
            }

            return RegionEndpoint.GetBySystemName(systemName);
        }

        public static string BuildS3BucketPath(string filePath, string originalRoot, string s3BucketName)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The filePath parameter must be a non-empty string", nameof(filePath));
            }

            if (string.IsNullOrEmpty(originalRoot))
            {
                throw new ArgumentException("The originalRoot parameter must be a non-empty string", nameof(originalRoot));
            }

            if (string.IsNullOrEmpty(s3BucketName))
            {
                throw new ArgumentException("The s3BucketName parameter must be a non-empty string", nameof(s3BucketName));
            }
            //replacing root folder with s3 bucket name
            filePath = filePath.Replace(originalRoot, s3BucketName, StringComparison.InvariantCultureIgnoreCase);

            //replacing all \ characters into /
            filePath = filePath.Replace('\\', '/');

            //removing last / character in filePath, the filePath should only display like this acu-bucketname/DOCUMENTS/ADF2_CAAPS/LIVE/2020/01/01
            if (filePath.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                filePath = filePath.Remove(filePath.Length - 1);

            return filePath;
        }
    }
}