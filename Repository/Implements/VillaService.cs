using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;

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

        public async Task<Villa> UpdateVilla(string name, Villa villa)
        {
            try
            {
                Villa villaRequest = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Name.ToLower() == name.ToLower() ) ??
                    throw new ArgumentException($"Villa {name} no encontrada");
                var validVilla = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Name.ToLower() == villa.Name.ToLower() );
                if (validVilla != null && villa.Name.ToLower() != name.ToLower())
                {
                    throw new ArgumentException($"El nombre {villa.Name} no está disponible.");
                }
                villa.UpdateDate = DateTime.Now;
                villa.CreationDate = villaRequest.CreationDate;
                villa.Id = villaRequest.Id;
                _dbContext.Villas.Update(villa);
                await _dbContext.SaveChangesAsync();
                return villa;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Villa> UpdatePartialVilla(string name, JsonPatchDocument<VillaDTO> villa)
        {
            try
            {
                Villa villaSrc = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Name.ToLower() == name.ToLower()) ??
                    throw new ArgumentException($"No se encontró un usuario con Name {name}.", nameof(name));
                VillaDTO villaDTO = _mapper.Map<VillaDTO>(villaSrc);
                villa.ApplyTo(villaDTO);
                Villa villaUpdate = _mapper.Map<Villa>(villaDTO);
                villaUpdate.UpdateDate = DateTime.Now;
                villaUpdate.CreationDate = villaSrc.CreationDate;
                villaUpdate.Id = villaSrc.Id;
                _dbContext.Villas.Update(villaUpdate);
                await _dbContext.SaveChangesAsync();
                return villaUpdate;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
