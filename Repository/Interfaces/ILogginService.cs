using MagicVillaApi.DTOs;
using MagicVillaApi.Models;

namespace MagicVillaApi.Repository.Interfaces
{
    public interface ILogginService
    {
        public Task<User> GetUserLoggin(UserLoginDTO userLoginDTO);
        public Task<User> UpdatePassword(UserLoginDTO userLoginDTO);
    }
}
