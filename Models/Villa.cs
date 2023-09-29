using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MagicVillaApi.Models
{
    public class Villa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        public double Tariff { get; set; }
        public int NumberOfOccupants { get; set; }
        public int SquareMeter { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
