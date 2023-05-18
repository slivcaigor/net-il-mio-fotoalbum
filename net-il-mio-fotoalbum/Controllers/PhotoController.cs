using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

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


        [HttpGet]
        public IActionResult Details(int id)
        {
            using PhotoContext db = new();
            Photo? photo = db.Photo
                .Where(photo => photo.Id == id)
                .Include(photo => photo.Categories)
               .FirstOrDefault();

            if (photo != null)
            {
                return View("Details", photo);
            }
            return RedirectToAction("Error404");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            using PhotoContext db = new();
            PhotoFormModel model = new();
            List<Category> categories = db.Categories.ToList();

            List<SelectListItem> listCategories = new();

            foreach (Category category in categories)
            {
                listCategories.Add(new SelectListItem()
                { Text = category.Name, Value = category.Id.ToString() });
            }


            model.Photo = new Photo();
            model.Category = listCategories;

            return View("Create", model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoFormModel data)
        {
            if (!ModelState.IsValid)
            {
                using PhotoContext context = new();

                List<Category> categories = context.Categories.ToList();
                List<SelectListItem> listCategories = new();

                foreach (Category category in categories)
                {
                    listCategories.Add(new SelectListItem()
                    { Text = category.Name, Value = category.Id.ToString() });
                }

                data.Category = listCategories;

                return View(data);
            }

            using PhotoContext db = new();
            Photo pizza = new Photo
            {
                Title = data.Photo.Title,
                Description = data.Photo.Description,
                Categories = new List<Category>()
            };

            if (data.ImageFile != null && data.ImageFile.Length > 0)
            {
                string imageUrl = UploadToS3(data.ImageFile);
                pizza.Image = imageUrl;
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Seleziona un'immagine");
                return View(data);
            }

            if (data.SelectedCategories != null)
            {
                foreach (string selectedCategoryId in data.SelectedCategories)
                {
                    if (selectedCategoryId != null && int.TryParse(selectedCategoryId, out int categoryId))
                    {
                        Category category = db.Categories.FirstOrDefault(ing => ing.Id == categoryId);
                        if (category != null)
                        {
                            pizza.Categories.Add(category);
                        }
                    }
                }
            }

            db.Photo.Add(pizza);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            using PhotoContext db = new();
            PhotoFormModel model = new();
            Photo photo = db.Photo.Include(p => p.Categories).FirstOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return NotFound();
            }

            List<Category> categories = db.Categories.ToList();
            List<SelectListItem> listCategories = new();

            foreach (Category category in categories)
            {
                listCategories.Add(new SelectListItem()
                { Text = category.Name, Value = category.Id.ToString() });
            }

            model.Photo = photo;
            model.Category = listCategories;
            model.SelectedCategories = photo.Categories.Select(c => c.Id.ToString()).ToList();

            return View("Update", model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, PhotoFormModel data)
        {
            if (!ModelState.IsValid)
            {
                using PhotoContext context = new();

                List<Category> categories = context.Categories.ToList();
                List<SelectListItem> listCategories = new();

                foreach (Category category in categories)
                {
                    listCategories.Add(new SelectListItem()
                    { Text = category.Name, Value = category.Id.ToString() });
                }

                data.Category = listCategories;

                return View(data);
            }

            using PhotoContext db = new();
            Photo photo = db.Photo.Include(p => p.Categories).FirstOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return NotFound();
            }

            photo.Title = data.Photo.Title;
            photo.Description = data.Photo.Description;
            photo.Categories.Clear();

            if (data.ImageFile != null && data.ImageFile.Length > 0)
            {
                string imageUrl = UploadToS3(data.ImageFile);
                photo.Image = imageUrl;
            }

            if (data.SelectedCategories != null)
            {
                foreach (string selectedCategoryId in data.SelectedCategories)
                {
                    if (selectedCategoryId != null && int.TryParse(selectedCategoryId, out int categoryId))
                    {
                        Category category = db.Categories.FirstOrDefault(c => c.Id == categoryId);
                        if (category != null)
                        {
                            photo.Categories.Add(category);
                        }
                    }
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using PhotoContext db = new();
            Photo? photo = db.Photo.Where(photo => photo.Id == id).FirstOrDefault();

            if (photo != null)
            {
                db.Photo.Remove(photo);

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error404");
            }
        }
    }
}
