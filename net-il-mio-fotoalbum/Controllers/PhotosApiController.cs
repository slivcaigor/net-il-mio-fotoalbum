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
        public IActionResult GetPhotos(string? search, int page = 1, int pageSize = 6)
        {
            using (var context = new PhotoContext())
            {
                IQueryable<Photo> photos = context.Photo;

                if (!string.IsNullOrEmpty(search))
                {
                    photos = photos.Where(p => p.Title.ToLower().Contains(search.ToLower()));
                }

                int totalCount = photos.Count();
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                photos = photos.Skip((page - 1) * pageSize).Take(pageSize);

                var result = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Photos = photos.ToList()
                };

                return Ok(result);
            }
        }

    }
}
