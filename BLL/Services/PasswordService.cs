using System.Security.Cryptography;
using System.Text;
using BLL.Interfaces;

namespace BLL.Services
{
    public class PasswordService : IPasswordService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 150000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        public string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                Algorithm,
                HashSize
            );

            return $"pbkdf2.{Algorithm.Name.ToLower()}.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 5 || parts[0] != "pbkdf2")
                return false;

            try
            {
                var algorithm = parts[1];
                var iterations = int.Parse(parts[2]);
                byte[] salt = Convert.FromBase64String(parts[3]);
                byte[] originalHash = Convert.FromBase64String(parts[4]);

                if (algorithm != Algorithm.Name.ToLower())
                    return false;

                byte[] testHash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    salt,
                    iterations,
                    Algorithm,
                    HashSize
                );

                return CryptographicOperations.FixedTimeEquals(originalHash, testHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
