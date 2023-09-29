using MagicVillaApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MagicVillaApi.DTOs
{
    public class NumberVillaDTO
    {
        [Required]
        public int NumVilla { get; set; }
        [Required]
        public int IdVilla { get; set; }
        public string DetailsVilla { get; set; }
    }
}
