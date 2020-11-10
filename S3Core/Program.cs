using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace S3Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                UploadBase64(Helper.GetFileInBase64()).Wait();

                UploadBytes(Helper.GetFileInBytes()).Wait();

                UploadPath(Helper.GetPath()).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail:Exception: " + ex.Message);
            }
        }

        private async static Task UploadBase64(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            await Upload(ms);
        }

        private async static Task UploadBytes(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            await Upload(ms);
        }

        private async static Task UploadPath(string path)
        {
            var bytes = File.ReadAllBytes(path);
            using var ms = new MemoryStream(bytes);
            await Upload(ms);
        }

        /* main method for uploading to S3 */
        private async static Task Upload(MemoryStream ms)
        {
            var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var transfer = new TransferUtility(s3Client);

            await transfer.UploadAsync(new TransferUtilityUploadRequest
            {
                BucketName = "edmar-bucket-test",
                FilePath = "",
                Key = Guid.NewGuid() + ".jpg",
                InputStream = ms,
            });

            s3Client.Dispose();
        }
    }
}
