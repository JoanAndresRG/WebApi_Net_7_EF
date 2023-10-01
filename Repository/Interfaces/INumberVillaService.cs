using MagicVillaApi.Models;
using MagicVillaApi.Repository.Intefaces;

namespace MagicVillaApi.Repository.Interfaces
{
    public interface INumberVillaService : IRepositoryService<NumberVilla>
    {
        public Task<NumberVilla> UpdateVilla(NumberVilla numberVilla); 
    }
}
