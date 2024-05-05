using System.Security.Cryptography;
using System.Text;

namespace StreamingPlatform.Utils
{
    /// <summary>
    /// Helper class to encrypt and decrypt data using AES encryption. This will be used to encrypt and decrypt secure data in the application.
    /// </summary>
    public static class SecureDataEncryptionHelper
    {
        /// <summary>
        /// The encryption key used to encrypt and decrypt data.
        /// </summary>
        public static string? Key { get; private set; }

        /// <summary>
        /// Encrypts the specified data using AES encryption.
        /// </summary>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <returns>The encrypted data as a Base64-encoded string.</returns>
        public static string Encrypt(string dataToEncrypt)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(Key, "Encryption key");

            if (string.IsNullOrEmpty(dataToEncrypt) || string.IsNullOrWhiteSpace(dataToEncrypt))
            {
                return string.Empty;
            }

            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream memoryStream = new();
                using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new(cryptoStream))
                {
                    streamWriter.Write(dataToEncrypt);
                }

                encrypted = memoryStream.ToArray();
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts the specified data using AES decryption.
        /// </summary>
        /// <param name="dataToDecrypt">The encrypted data to decrypt (Base64-encoded string).</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string Decrypt(string dataToDecrypt)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(Key, "Encryption key");

            if (string.IsNullOrEmpty(dataToDecrypt) || string.IsNullOrWhiteSpace(dataToDecrypt))
            {
                return string.Empty;
            }

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform descriptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] buffer = Convert.FromBase64String(dataToDecrypt);
            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, descriptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Sets the encryption key used for encryption and decryption.
        /// </summary>
        /// <param name="key">The encryption key.</param>
        public static void SetEncryptionKey(string key)
        {
            Key = key;
        }
    }
}
