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
                if (string.IsNullOrWhiteSpace(userLoginDTO.UserName) || string.IsNullOrWhiteSpace(userLoginDTO.Password) )
                    throw new ArgumentNullException(nameof(userLoginDTO), "Todos los campos son obligatorios");
                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userLoginDTO.UserName) ??
                    throw new ArgumentException(nameof(user), "El usuario ingresado no existe");
                string passwordDesencryt = await _encryptPasswordUser.DesEncryptPass(user.EncryptedPassword);
                if (userLoginDTO.Password != passwordDesencryt)
                    throw new ArgumentException(nameof(user), "Credenciales invalidas");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<User> UpdateCredentials(UserLogginDTOUpdate userLogginDTOUpdate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userLogginDTOUpdate.OldUserName) ||
                    string.IsNullOrWhiteSpace(userLogginDTOUpdate.NewUserName) ||
                    string.IsNullOrWhiteSpace(userLogginDTOUpdate.OldPassword) ||
                    string.IsNullOrWhiteSpace(userLogginDTOUpdate.NewPassword))
                {
                    throw new ArgumentNullException(nameof(userLogginDTOUpdate), "Todos los campos son obligatorios");
                }
                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userLogginDTOUpdate.OldUserName) ??
                    throw new ArgumentException(nameof(user), "El usuario ingresado no existe");
                string passwordDesencryt = await _encryptPasswordUser.DesEncryptPass(user.EncryptedPassword);
                if (userLogginDTOUpdate.OldPassword != passwordDesencryt)
                    throw new ArgumentException(nameof(user), "Credenciales invalidas");
                user.UserName = userLogginDTOUpdate.NewUserName;
                user.EncryptedPassword = await _encryptPasswordUser.EncryptPass(userLogginDTOUpdate.NewPassword);
                user.UpdateDate = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
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
