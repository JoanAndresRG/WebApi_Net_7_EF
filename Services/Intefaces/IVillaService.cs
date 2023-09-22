using MagicVillaApi.Models.Class;
using MagicVillaApi.Models.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace MagicVillaApi.Services.Intefaces
{
    public interface IVillaService
    {
        public Task<List<Villa>> GetVillas();
        public Task<Villa> GetVilla(int id);
        public Task CreateVilla(Villa villa);
        public Task DeleteVilla(int id);
        public Task<Villa> UpdateVilla(Villa villa);
        public Task UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> villa);
    }
}
