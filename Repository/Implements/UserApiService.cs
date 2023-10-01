using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using MagicVillaApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Repository.Implements
{
    public class UserApiService : IUserApiService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserApiService> _logger;
        private readonly IMapper _mapper;
        private readonly IEncryptPasswordUser _encryptPasswordUser;
        public UserApiService(ApplicationDbContext dbContext, ILogger<UserApiService> logger, IMapper mapper, IEncryptPasswordUser encryptPasswordUser)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
            _encryptPasswordUser = encryptPasswordUser;
        }

        public async Task<User> CreateUser(User user)
        {
            try
            {
                // Encriptar la contraseña antes de guardarla
                User existUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
                if (existUser != null) 
                    throw new Exception("El nombre del usuario ingresado no esta disponible");
                user.EncryptedPassword = await _encryptPasswordUser.EncryptPass(user.Password);
                user.UserRol = ((RolUser)int.Parse(user.UserRol)).GetRoleName();
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                _logger.LogError($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task DeleteUser(int idUser)
        {
            try
            {
                if (idUser <= 0) 
                    throw new ArgumentException($"El id ingresado no es valido {idUser}.");
                User user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == idUser) ?? throw new ArgumentException($"No se encontró un usuario con Id {idUser}.", nameof(idUser));
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetUser(int idUser)
        {
            try
            {
                if (idUser <= 0) 
                    throw new ArgumentException($"El id ingresado no es valido {idUser}.");
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == idUser);
                return user ?? 
                    throw new ArgumentException($"No se encontró un usuario con Id {idUser}.", nameof(idUser));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            try
            {
                List<User> users = new();
                users = await _dbContext.Users.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<User> UpdateUser(User user)
        {
            try
            {
                if (user == null) 
                    throw new ArgumentNullException(nameof(user), "Debe ingresar todos los parametros del uasurio.");
                User userSearch = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id) ?? 
                    throw new ArgumentException($"No se encontró un usuario con Id {user.Id}.", nameof(user.Id));
                userSearch.UpdateDate = DateTime.Now;
                userSearch.UserRol = ((RolUser)int.Parse(user.UserRol)).GetRoleName();
                userSearch.UserName = user.UserName;
                userSearch.Email = user.Email;
                _dbContext.Users.Update(userSearch);
                await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
