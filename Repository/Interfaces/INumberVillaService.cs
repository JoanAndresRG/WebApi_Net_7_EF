using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Intefaces;
using Microsoft.AspNetCore.JsonPatch;

namespace MagicVillaApi.Repository.Interfaces
{
    public interface INumberVillaService : IRepositoryService<NumberVilla>
    {
        public Task<NumberVilla> UpdateVilla(NumberVilla numberVilla); 
    }
}
