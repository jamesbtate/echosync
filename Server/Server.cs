using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter a password to be hashed: ");
                string password = Console.ReadLine();
                RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
                byte[] salt = new byte[16];
                rng.GetNonZeroBytes(salt);
                Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(password, salt, 4096);
                Console.WriteLine(System.Convert.ToBase64String(hash.GetBytes(256)));
            }
        }
    }
}
