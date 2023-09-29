using System.ComponentModel.DataAnnotations;

namespace MagicVillaApi.DTOs
{
    public class VillaDTO
    {

        [Required]
        [MaxLength(30, ErrorMessage = "La cantidad maxima de caracteres es de 30.")]
        public string Name { get; set; }

        public string Details { get; set; }
        [Required(ErrorMessage = "Campo tarifa es requerido.")]
        public double Tariff { get; set; }

        [Required(ErrorMessage = "Campo número de habitantes requerido.")]
        [Range(1, 1000000, ErrorMessage = "La cantidad no cumple el rango.")]
        public int NumberOfOccupants { get; set; }

        [Required(ErrorMessage = "Campo metros cuadrados requerido.")]
        [Range(10, 10000, ErrorMessage = "La cantidad no cumple el rango.")]
        public int SquareMeter { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }

    }
}
