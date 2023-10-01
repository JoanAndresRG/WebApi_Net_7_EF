using MagicVillaApi.Data;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Repository.Implements
{
    public class LogginService : ILogginService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LogginService> _logger;
        private readonly IEncryptPasswordUser _encryptPasswordUser;
        public LogginService(ILogger<LogginService> logger, ApplicationDbContext context, IEncryptPasswordUser encryptPasswordUser)
        {
            _logger = logger;
            _context = context;
            _encryptPasswordUser = encryptPasswordUser;
        }

        public async Task<User> GetUserLoggin(UserLoginDTO userLoginDTO)
        {
            try
            {
                if (userLoginDTO.UserName == "" || userLoginDTO.Password == "") 
                    throw new ArgumentNullException(nameof(userLoginDTO), "Todos los campos son obligatorios");
                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userLoginDTO.UserName);
                if (user == null) 
                    throw new ArgumentException(nameof(user), "El usuario ingresado no existe");
                string passwordDesencryt = await _encryptPasswordUser.DesEncryptPass(user.EncryptedPassword);
                if ( userLoginDTO.Password != passwordDesencryt ) 
                    throw new ArgumentException(nameof(user), "Credenciales invalidas");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }

        public Task<User> UpdatePassword(UserLoginDTO userLoginDTO)
        {
            throw new NotImplementedException();
        }
    }
}
