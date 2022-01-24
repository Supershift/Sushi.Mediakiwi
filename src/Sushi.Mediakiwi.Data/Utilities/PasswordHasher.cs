using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Sushi.Mediakiwi.Data.Utilities
{
    public sealed class PasswordHasher<T> : IPasswordHasher<T> where T : class, IProfile
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 1000;


        public string HashPassword(T user, string password)
        {
            string pwd = "";

            using (var algorithm = new Rfc2898DeriveBytes(
       password,
       SaltSize,
       Iterations,
       HashAlgorithmName.SHA512))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);
                pwd = $"{Iterations}.{salt}.{key}";
            }

            return pwd;
        }

        public PasswordVerificationResult VerifyHashedPassword(T user, string hashedPassword, string providedPassword)
        {
            var parts = hashedPassword.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var needsUpgrade = iterations != Iterations;

            using (var algorithm = new Rfc2898DeriveBytes(
              providedPassword,
              salt,
              iterations,
              HashAlgorithmName.SHA512))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);
                var verified = keyToCheck.SequenceEqual(key);

                if (verified)
                {
                    if (needsUpgrade)
                    {
                        return PasswordVerificationResult.SuccessRehashNeeded;
                    }
                    else 
                    {
                        return PasswordVerificationResult.Success;
                    }
                }
                else 
                {
                    return PasswordVerificationResult.Failed;
                }
            }
        }
    }
}
