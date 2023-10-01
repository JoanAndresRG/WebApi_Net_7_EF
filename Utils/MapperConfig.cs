using AutoMapper;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;

namespace MagicVillaApi.Utils
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            #region Villas
            CreateMap<Villa, VillaDTO>().ReverseMap();
            #endregion

            #region NumVilla
            CreateMap<NumberVilla, NumberVillaDTO>().ReverseMap();
            #endregion

            #region User
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserDtoCreate>().ReverseMap();
            CreateMap<User, UserDtoUpdate>().ReverseMap();
            #endregion
        }
    }
}
