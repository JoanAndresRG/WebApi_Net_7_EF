using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVillaApi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public string EncryptedPassword { get; set; } // Este campo se mapea a la base de datos
        public string Email { get; set; }
        public string UserRol { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public String? Token { get; set; }
    }
}
