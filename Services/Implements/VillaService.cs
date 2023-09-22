using MagicVillaApi.Data;
using MagicVillaApi.Models.Class;
using MagicVillaApi.Models.DTOs;
using MagicVillaApi.Services.Intefaces;
using MagicVillaApi.Utils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Services.Implements
{
    public class VillaService : IVillaService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<VillaService> _logger;
        public VillaService(ApplicationDbContext applicationDbContext, ILogger<VillaService> logger)
        {
            _dbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task CreateVilla(Villa villa)
        {
            try
            {
                await _dbContext.Villas.AddAsync(villa);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task DeleteVilla(int id)
        {
            try
            {
                var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
                if (villa == null)
                {
                    _logger.LogWarning("Villa con id " + id + " no encontrada.");
                    throw new ArgumentNullException();
                }
                _dbContext.Villas.Remove(villa);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<Villa> GetVilla(int id)
        {
            try
            {
                var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
                return villa;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<List<Villa>> GetVillas()
        {
            try
            {
                List<Villa> villas = await _dbContext.Villas.ToListAsync();
                villas.ForEach(villa => { _logger.LogInformation($"Id: {villa.Id} - Villa: {villa.Name} \n"); });
                return villas;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }

        }

        public async Task UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> villa)
        {
            try
            {
                VillaDTO villaSrch = (await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id)).ConvertVillaDTO();
                villa.ApplyTo(villaSrch);
                Villa newVilla = new()
                {
                    Id = id,
                    Name = villaSrch.Name,
                    Tariff = villaSrch.Tariff,
                    Details = villaSrch.Details,
                    ImageUrl = villaSrch.ImageUrl,
                    Amenity = villaSrch.Amenity,
                    NumberOfOccupants = villaSrch.NumberOfOccupants,
                    SquareMeter = villaSrch.SquareMeter,
                };
                _dbContext.Villas.Update(newVilla);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }

        }

        public async Task<Villa> UpdateVilla(Villa villa)
        {
            try
            {
                _dbContext.Villas.Update(villa);
                await _dbContext.SaveChangesAsync();
                return villa;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
