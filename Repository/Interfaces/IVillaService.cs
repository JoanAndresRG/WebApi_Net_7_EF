using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Intefaces;
using Microsoft.AspNetCore.JsonPatch;

namespace MagicVillaApi.Repository.Interfaces
{
    public interface IVillaService : IRepositoryService<Villa>
    {
        public Task<Villa> UpdateVilla(string name, Villa villa); 
        public Task<Villa> UpdatePartialVilla(string name, JsonPatchDocument<VillaDTO> villa); 
    }
}
