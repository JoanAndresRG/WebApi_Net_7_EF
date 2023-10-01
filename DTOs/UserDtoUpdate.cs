using MagicVillaApi.Utils;
using System.ComponentModel.DataAnnotations;

namespace MagicVillaApi.DTOs
{
    public class UserDtoUpdate
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Range(0, (int)RolUser.Guest, ErrorMessage = "Rol no valido")]
        public int UserRol
        {
            get; set;
        }
    }
}
