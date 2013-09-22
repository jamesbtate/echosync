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

        public static string HashPasswordBase64(string password)
        {
            byte[] salt = new byte[16];
            rng.GetNonZeroBytes(salt);
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(password, salt, 4096);
            return System.Convert.ToBase64String(hash.GetBytes(256));
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
