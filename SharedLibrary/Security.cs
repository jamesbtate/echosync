using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SharedLibrary
{
    public static class Security
    {
        static RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();

        public static string HashPasswordBase64(string password)
        {
            byte[] salt = new byte[16];
            rng.GetNonZeroBytes(salt);
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(password, salt, 4096);
            return System.Convert.ToBase64String(hash.GetBytes(256));
        }
    }
}
