using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        private readonly IConfiguration _configuration;

        public PhotoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string UploadToS3(IFormFile file)
        {
            var accessKey = _configuration["AWSCredentials:AccessKey"];
            var secretKey = _configuration["AWSCredentials:SecretKey"];
            var region = _configuration["AWSCredentials:AWSRegion"];
            var bucketName = _configuration["AWSCredentials:S3BucketName"];

            var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
            var fileTransferUtility = new TransferUtility(s3Client);

            using var stream = file.OpenReadStream();
            var filePath = $"foto/{file.FileName}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = filePath,
                InputStream = stream,
            };

            fileTransferUtility.Upload(uploadRequest);

            return $"https://{bucketName}.s3.amazonaws.com/{filePath}";
        }
        [HttpGet]
        public IActionResult Index(int pageNumber = 1, int pageSize = 6)
        {
            using PhotoContext db = new();
            List<Photo> photos = db.Photo
                .OrderBy(photo => photo.Id)
                .Include(photo => photo.Categories)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (photos.Count == 0)
            {
                ViewBag.Message = "No photos available at the moment!";
            }

            int totalPhotos = db.Photo.Count();
            ViewData["TotalPhotos"] = totalPhotos;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = pageNumber;

            return View("Index", photos);
        }

    }
}
