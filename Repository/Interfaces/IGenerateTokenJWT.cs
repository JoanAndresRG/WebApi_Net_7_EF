using MagicVillaApi.Models;

namespace MagicVillaApi.Repository.Interfaces
{
    public interface IGenerateTokenJWT
    {
        public Task<User> GenerateToken(User user);
    }
}
