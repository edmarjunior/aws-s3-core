using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
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
            /* uncomment the lines below that you want to test */

            try
            {
                //UploadBase64(Helper.GetFileInBase64()).Wait();

                //UploadBytes(Helper.GetFileInBytes()).Wait();

                //UploadPath(Helper.GetPathImage()).Wait();

                //GetUrlWithExpires();

                //MakeUrl();

                // DownloadFile().Wait();

                // GetFileInBytes().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail:Exception: " + ex.Message);
            }
        }

        /* ******************************* Upload *********************************************/
        #region #Upload Methods
        
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
            using var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var transfer = new TransferUtility(s3Client);

            await transfer.UploadAsync(new TransferUtilityUploadRequest
            {
                BucketName = "edmar-bucket-test",
                FilePath = "",
                Key = Guid.NewGuid() + ".jpg",
                InputStream = ms,
            });
        }

        #endregion



        /* ******************************* Download *********************************************/
        #region #Download Methods

        private static void GetUrlWithExpires()
        {
            using var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = "edmar-bucket-test",
                Key = "image.jpg",
                Expires = DateTime.UtcNow.AddMinutes(1),
            });

            Console.WriteLine("URL: " + url);
        }

        private static void MakeUrl()
        {
            var bucketName = "edmar-bucket-test";
            var keyName = "image.jpg";
            var url = $"https://{bucketName}.s3.amazonaws.com/{keyName}";

            Console.WriteLine("URL: " + url);
        }

        private async static Task DownloadFile()
        {
            using var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var transfer = new TransferUtility(s3Client);

            var localPath = "C:/teste/file-s3.jpg";

            await transfer.DownloadAsync(localPath, "edmar-bucket-test", "image.jpg");
        }

        private static async Task<byte[]> GetFileInBytes()
        {
            using var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var response = await s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = "edmar-bucket-test",
                Key = "image.jpg"
            });

            using var responseStream = response.ResponseStream;
            
            using var memoryStream = new MemoryStream();

            await responseStream.CopyToAsync(memoryStream);

            var bytes = memoryStream.GetBuffer();

            return bytes;
        }

        #endregion
    }
}
