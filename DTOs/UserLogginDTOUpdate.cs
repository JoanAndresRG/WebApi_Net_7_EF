using System.ComponentModel.DataAnnotations;

namespace MagicVillaApi.DTOs
{
    public class UserLogginDTOUpdate
    {
        [Required]
        public string OldUserName { get; set; }

        [Required]
        public string NewUserName { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Número Maximo, 10 caracteres")]
        public string OldPassword { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Número Maximo, 10 caracteres")]
        public string NewPassword { get; set; }
    }
}
