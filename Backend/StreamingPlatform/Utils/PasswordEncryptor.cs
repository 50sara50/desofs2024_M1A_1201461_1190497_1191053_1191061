using System.Security.Cryptography;
using System.Text;

namespace StreamingPlatform.Utils
{
    /// <summary>
    /// Class to encrypt passwords using a secure hashing algorithm.
    /// </summary>
    public static class PasswordEncryptor
    {
        /// <summary>
        /// Encrypts the password using a secure hashing algorithm.
        /// </summary>
        /// <param name="password">The user's password.</param>
        /// <param name="salt">The salt generated for this password.</param>
        /// <param name="pepper">The secret pepper value.</param>
        /// <param name="iteration">The number of iterations to hash the password.</param>
        /// <returns>The hashed and encrypted password.</returns>
        public static string EncryptPassword(string password, string salt, string pepper, int iteration)
        {
            if (iteration <= 0)
            {
                return password;
            }

            string passwordSaltPepper = $"{password}{salt}{pepper}";
            byte[] byteValue = Encoding.UTF8.GetBytes(passwordSaltPepper);
            byte[] byteHash = SHA256.HashData(byteValue);
            string hash = Convert.ToBase64String(byteHash);
            return EncryptPassword(hash, salt, pepper, iteration - 1);
        }

        /// <summary>
        /// Generates a random salt for password hashing.
        /// </summary>
        /// <returns>The generated salt as a base64 string.</returns>
        public static string GenerateSalt()
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] byteSalt = new byte[16];
            rng.GetBytes(byteSalt);
            string salt = Convert.ToBase64String(byteSalt);
            return salt;
        }
    }
}
