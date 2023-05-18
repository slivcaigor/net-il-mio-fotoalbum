using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    [Table("Message")]
    public class Message
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("text")]
        public string Text { get; set; }
    }
}
