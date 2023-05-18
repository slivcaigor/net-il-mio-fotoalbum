using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhotosApiController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetPhotos(string? search)
        {
            using (var context = new PhotoContext())
            {
                IQueryable<Photo> photos = context.Photo;

                if (!string.IsNullOrEmpty(search))
                {
                    photos = photos.Where(p => p.Title.ToLower().Contains(search.ToLower()));
                }

                return Ok(photos.ToList());
            }
        }


    }
}
