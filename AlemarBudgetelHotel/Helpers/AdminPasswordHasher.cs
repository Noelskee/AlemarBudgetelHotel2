using System;
using System.Security.Cryptography;
using System.Text;

namespace AlemarBudgetelHotel.Helpers
{
    public class AdminPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Create the hash
            byte[] hash = GenerateHash(password, salt);

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Convert base64 string to byte array
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                // Extract salt
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Extract hash
                byte[] storedHash = new byte[HashSize];
                Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

                // Compute hash of provided password
                byte[] computedHash = GenerateHash(password, salt);

                // Compare hashes
                return CompareHashes(storedHash, computedHash);
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GenerateHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        private static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }
    }
}
