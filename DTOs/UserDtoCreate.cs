using MagicVillaApi.Utils;
using System.ComponentModel.DataAnnotations;

namespace MagicVillaApi.DTOs
{
    public class UserDtoCreate
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Número Maximo, 10 caracteres")]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Range(0, (int)RolUser.Guest, ErrorMessage = "Rol no valido")]
        public int UserRol { get; set; }

    }
}
