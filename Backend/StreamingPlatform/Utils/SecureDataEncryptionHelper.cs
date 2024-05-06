using System.Security.Cryptography;
using System.Text;

namespace StreamingPlatform.Utils
{
    /// <summary>
    /// Helper class to encrypt and decrypt data using AES encryption.
    /// </summary>
    public static class SecureDataEncryptionHelper
    {
        /// <summary>
        /// The block size for AES encryption in bits (128 bits = 16 bytes).
        /// </summary>
        private const int AesBlockSize = 128;

        /// <summary>
        /// The encryption key used for AES encryption and decryption.
        /// </summary>
        private static string? Key { get; set; }

        /// <summary>
        /// Encrypts the specified data using AES encryption.
        /// </summary>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <returns>The encrypted data as a Base64-encoded string.</returns>
        public static string Encrypt(string dataToEncrypt)
        {
            if (string.IsNullOrEmpty(Key))
            {
                throw new InvalidOperationException("Encryption key is not set.");
            }

            if (string.IsNullOrEmpty(dataToEncrypt))
            {
                return string.Empty;
            }

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            byte[] iv = GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncrypt);
            cryptoStream.Write(dataBytes, 0, dataBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] encryptedBytes = memoryStream.ToArray();
            byte[] result = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts the specified data using AES decryption.
        /// </summary>
        /// <param name="dataToDecrypt">The encrypted data to decrypt (Base64-encoded string).</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string Decrypt(string dataToDecrypt)
        {
            if (string.IsNullOrEmpty(Key))
            {
                throw new InvalidOperationException("Encryption key is not set.");
            }

            if (string.IsNullOrEmpty(dataToDecrypt))
            {
                return string.Empty;
            }

            byte[] encryptedBytes = Convert.FromBase64String(dataToDecrypt);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[AesBlockSize / 8];
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

            byte[] encryptedDataWithoutIV = new byte[encryptedBytes.Length - iv.Length];
            Buffer.BlockCopy(encryptedBytes, iv.Length, encryptedDataWithoutIV, 0, encryptedDataWithoutIV.Length);

            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream memoryStream = new(encryptedDataWithoutIV);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);

            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Sets the encryption key used for encryption and decryption.
        /// </summary>
        /// <param name="key">The encryption key.</param>
        public static void SetEncryptionKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Encryption key cannot be null or empty.");
            }

            Key = key;
        }

        /// <summary>
        /// Generates a new random initialization vector (IV) for AES encryption.
        /// </summary>
        /// <returns>The generated IV as a byte array.</returns>
        private static byte[] GenerateIV()
        {
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();
            return aes.IV;
        }
    }
}
