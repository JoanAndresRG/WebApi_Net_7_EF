using MagicVillaApi.Models.Class;
using MagicVillaApi.Models.DTOs;

namespace MagicVillaApi.Utils
{
    public static class Util
    {
        public static VillaDTO ConvertVillaDTO(this Villa villa)
        {
            if (villa != null)
            {
                return new VillaDTO
                {
                    Name = villa.Name,
                    Tariff = villa.Tariff,
                    Details = villa.Details,
                    Amenity = villa.Amenity,
                    ImageUrl = villa.ImageUrl,
                    NumberOfOccupants = villa.NumberOfOccupants,
                    SquareMeter = villa.SquareMeter,
                };
            }
            return null;
        }
    }
}
