using AutoMapper;
using MagicVillaApi.Models.Class;
using MagicVillaApi.Models.DTOs;

namespace MagicVillaApi.Utils
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Villa, VillaDTO>().ReverseMap();
        }
    }
}
