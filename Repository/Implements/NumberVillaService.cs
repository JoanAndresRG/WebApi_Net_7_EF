using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Repository.Implements
{
    public class NumberVillaService : RepositoryService<NumberVilla>, INumberVillaService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<NumberVillaService> _logger;
        private readonly IMapper _mapper;
        public NumberVillaService(ApplicationDbContext dbContext, ILogger<NumberVillaService> logger, IMapper mapper) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<NumberVilla> UpdateVilla(NumberVilla numberVilla)
        {
            try
            {
                numberVilla.UpdateDate = DateTime.Now;
                numberVilla.CreationDate = (await _dbContext.NumberVillas.AsNoTracking().FirstOrDefaultAsync(x => x.IdVilla == numberVilla.IdVilla)).CreationDate;
                _dbContext.NumberVillas.Update(numberVilla);
                await _dbContext.SaveChangesAsync();
                return numberVilla;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
