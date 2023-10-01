namespace MagicVillaApi.Repository.Interfaces
{
    public interface IEncryptPasswordUser
    {
        public Task<string> EncryptPass(string password);
        public Task<string> DesEncryptPass(string encrytPassword);
    }
}
