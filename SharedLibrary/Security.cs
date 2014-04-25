using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace SharedLibrary
{
    public static class Security
    {
        static RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
        // below is an array containing the hash of the GetMachineGUID() for every PC used by James so far
        //    when developing this application. It is used for authenticating dev computers until a more
        //    permanent dev environment with a database is set up.
        public static string[] listOfKnownDevPCs = { "$pbkdf2-sha1$4096$Eg9w4YZVMYYoQ93LomT80A==$FI7ZxkjzOu4JJUV0bOQbMA==" };

        public static string HashPasswordBase64(string password)
        {
            int iterations = 4096;
            int passBytes = 16;
            byte[] salt = new byte[passBytes];
            rng.GetNonZeroBytes(salt);
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(password, salt, iterations);
            String passString = HashPasswordBase64(password, salt, iterations, passBytes);
            String saltString = System.Convert.ToBase64String(salt);
            string returnString = "$pbkdf2-sha1$" + iterations + "$" + saltString + "$" + passString; 
            return returnString;
        }

        public static string HashPasswordBase64(string password, byte[] salt, int iterations, int passBytes)
        {
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(password, salt, iterations);
            String passString = System.Convert.ToBase64String(hash.GetBytes(passBytes));
            return passString;
        }

        public static bool VerifyPasswordAgainstHash(string password, string base64Hash)
        {
            string[] hashParts = base64Hash.Split('$');
            if (hashParts[1] != "pbkdf2-sha1")
            {
                Logger.Error("Found unusual password hash algorithm: " + hashParts[1]);
                return false;
            }
            int iterations = Int32.Parse(hashParts[2]);
            string passHash = HashPasswordBase64(password, System.Convert.FromBase64String(hashParts[3]), iterations, 16);
            return passHash.Equals(hashParts[4]);
        }

        public static bool ManuallyVerifyCA(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("ManuallyVerifyCA");
            bool isValid = false;
            if (sslPolicyErrors == SslPolicyErrors.None) return true;
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch) return true;
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || (int)sslPolicyErrors == (int)SslPolicyErrors.RemoteCertificateNameMismatch + (int)SslPolicyErrors.RemoteCertificateChainErrors)
            {
                try
                {
                    X509Chain chain0 = new X509Chain();
                    chain0.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    // add all your extra certificate chain
                    chain0.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                    Console.WriteLine("tits buckets");
                    chain0.ChainPolicy.ExtraStore.Add(new X509Certificate2("..\\..\\..\\ca.p7b"));
                    Console.WriteLine("piss buckets");
                    isValid = chain0.Build((X509Certificate2)certificate);
                    if (isValid) return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("sslPolicyErrors: {0}", e.Message);
                    return false;
                }
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }

        private static void doStuff()
        {
            //X509Certificate2 x = new X509Certificate2(
        }
    }
}
