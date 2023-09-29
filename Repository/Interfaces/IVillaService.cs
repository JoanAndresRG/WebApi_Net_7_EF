using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Intefaces;
using Microsoft.AspNetCore.JsonPatch;

namespace MagicVillaApi.Repository.Interfaces
{
    public interface IVillaService : IRepositoryService<Villa>
    {
        public Task<Villa> UpdateVilla(Villa villa); 
        public Task UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> villa); 
    }
}
