using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;
using System.Data;
using Amazon;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Controllers
{
    public class AdminController : Controller
    {

        [HttpGet]
        public IActionResult Index(int pageNumber = 1, int pageSize = 6)
        {
            using (var db = new PhotoContext())
            {
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Messages()
        {
            using PhotoContext db = new();
            List<Message> messages = db.Messages.ToList();
            return View(messages);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteMessage(int id)
        {
            using (PhotoContext db = new PhotoContext())
            {
                Message message = db.Messages.Find(id);
                if (message != null)
                {
                    db.Messages.Remove(message);
                    db.SaveChanges();
                }
                return RedirectToAction("Messages");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Categories()
        {
            using (var db = new PhotoContext())
            {
                List<Category> categories = db.Categories.ToList();
                return View(categories);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var db = new PhotoContext())
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                }

                return RedirectToAction("Categories");
            }

            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCategory(int id)
        {
            using (var db = new PhotoContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id == id);
                if (category != null)
                {
                    db.Categories.Remove(category);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Categories");
        }

    }
}
