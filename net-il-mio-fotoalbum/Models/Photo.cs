using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    [Table("photos")]
    public class Photo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required")]
        [Column("title")]
        [MaxLength(32)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "The description is required")]
        [Column("description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Column("visibility")]
        public bool Visible { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        public List<Category>? Categories { get; set; }
    }
}
