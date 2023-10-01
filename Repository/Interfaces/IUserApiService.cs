using MagicVillaApi.Models;

namespace MagicVillaApi.Repository.Interfaces
{
    public interface IUserApiService
    {
        public Task<User> CreateUser(User user); 
        public Task<User> GetUser(int idUser);
        public Task<IEnumerable<User>> GetUsers();
        public Task<User> UpdateUser(User user);
        public Task DeleteUser(int idUser);
    }
}
