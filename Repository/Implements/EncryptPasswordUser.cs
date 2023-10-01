using MagicVillaApi.Repository.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MagicVillaApi.Repository.Implements
{
    public class EncryptPasswordUser : IEncryptPasswordUser
    {
        private readonly string _encryptionKey; // Clave secreta para el cifrado

        public EncryptPasswordUser(IConfiguration configuration)
        {
            // Obtén la clave secreta de la configuración
            _encryptionKey = configuration.GetSection("JWT:ClaveSecreta").Value;
        }

        #region ENCRYPT

        // Método para encriptar la contraseña
        public async Task<string> EncryptPass(string password)
        {
            using (Aes aesAlg = Aes.Create() ) 
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aesAlg.IV = new byte[16]; // IV (Vector de inicialización) aleatorio

                ICryptoTransform encryptor =  aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            await swEncrypt.WriteAsync(password);
                        }
                    }
                    return  Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            
        }
    
        #endregion

        #region DESENCRYPT
        public async Task<string> DesEncryptPass(string encrytPassword)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aesAlg.IV = new byte[16]; // IV (Vector de inicialización) aleatorio

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encrytPassword)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return await srDecrypt.ReadToEndAsync();
                        }
                    }
                }
            }
        }
        #endregion

    }
}
