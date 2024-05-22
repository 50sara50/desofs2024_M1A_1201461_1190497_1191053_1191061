using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Utils;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// The user's password.
    /// </summary>
    [Keyless]
    public class Password
    {
        public Password(string password, string pepper, int iterations = 3)
        {
            string salt = PasswordEncryptor.GenerateSalt();
            this.Salt = salt;
            this.Value = PasswordEncryptor.EncryptPassword(password, salt, pepper, iterations);
        }

        public Password()
        {
            this.Value = string.Empty;
            this.Salt = string.Empty;
        }

        /// <summary>
        /// The password's value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The password's hash.
        /// </summary>
        public string Salt { get; set; }
    }
}