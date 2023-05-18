using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoFormModel
    {
        public Photo Photo { get; set; }
        public List<SelectListItem>? Category { get; set; }
        public List<string>? SelectedCategories { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool Visibility { get; set; }
    }
}
