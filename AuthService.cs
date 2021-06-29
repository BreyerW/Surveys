using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Surveys.Model;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Surveys
{
    /// <summary>
    /// Interfejs do uwierzytelniania użytkowników
    /// </summary>
    public interface IAuthService
    {
        User Authenticate(string username, string password);
    }
    /// <summary>
    /// Klasa implementująca interfejs do uwierzytelniania użytkowników
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly surveyContext _context;
        public AuthService(surveyContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Metoda do uwierzytelniania i logowania użytkownika
        /// </summary>
        /// <param name="username">Podana nazwa użytkownika</param>
        /// <param name="password">Podane hasło</param>
        /// <returns></returns>
        public User Authenticate(string username, string password)
        {

            var user = _context.Users.SingleOrDefault(
                (Func<User, bool>)((u) =>
                {
                    const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
                    const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
                    const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
                    const int SaltSize = 128 / 8; // 128 bits
                    var bytes = Convert.FromBase64String(u.Password);
                    byte[] salt = new byte[SaltSize];
                    Buffer.BlockCopy(bytes, 0, salt, 0, salt.Length);
                    byte[] expectedSubkey = new byte[Pbkdf2SubkeyLength];
                    Buffer.BlockCopy(bytes, salt.Length, expectedSubkey, 0, expectedSubkey.Length);
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: password,
                        salt: salt,
                        prf: Pbkdf2Prf,
                        iterationCount: Pbkdf2IterCount,
                        numBytesRequested: Pbkdf2SubkeyLength));

                    return u.Username == username && Convert.ToBase64String(expectedSubkey) == hashed;
                }
                ));

            if (user == null)
                return null;

            return user;
        }
    }
}
