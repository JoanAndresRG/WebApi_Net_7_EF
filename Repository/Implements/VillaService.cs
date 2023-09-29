using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Repository.Implements
{
    public class VillaService : RepositoryService<Villa>, IVillaService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<VillaService> _logger;
        private readonly IMapper _mapper;

        public VillaService(ApplicationDbContext dbContext, ILogger<VillaService> logger, IMapper mapper) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;   
            _mapper = mapper;
        }

        public async Task<Villa> UpdateVilla(Villa villa)
        {
            try
            {
                villa.UpdateDate = DateTime.Now;
                villa.CreationDate = (await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == villa.Id)).CreationDate;    
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

        public async Task UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> villa)
        {
            try
            {
                Villa villaSrc = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
                VillaDTO villaDTO = _mapper.Map<VillaDTO>(villaSrc);
                villa.ApplyTo(villaDTO);
                Villa villaUpdate = _mapper.Map<Villa>(villaDTO);
                villaUpdate.UpdateDate = DateTime.Now;
                villaUpdate.CreationDate = villaSrc.CreationDate;
                villaUpdate.Id = villaSrc.Id;
                _dbContext.Villas.Update(villaUpdate);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
