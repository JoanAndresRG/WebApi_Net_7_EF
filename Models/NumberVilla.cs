using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVillaApi.Models
{
    public class NumberVilla
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NumVilla { get; set; }
        [Required]
        public int IdVilla { get; set; }
        [ForeignKey("IdVilla")]
        public Villa villa { get; set; }
        public string DetailsVilla { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
