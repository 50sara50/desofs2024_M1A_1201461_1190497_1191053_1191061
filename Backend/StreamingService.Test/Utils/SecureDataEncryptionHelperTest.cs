using Microsoft.Extensions.Configuration;
using StreamingPlatform.Dao;
using StreamingPlatform.Utils;

namespace StreamingService.Test.Utils
{
    [TestClass]
    public class SecureDataEncryptionHelperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            // the type specified here is just so the secrets library can 
            // find the UserSecretId we added in the csproj file
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddUserSecrets<StreamingDbContext>();
            IConfigurationRoot configuration = builder.Build();
            string encryptionKey = configuration.GetValue<string>("Keys:SecureDataKey") ?? PasswordEncryptor.GenerateSalt();
            SecureDataEncryptionHelper.SetEncryptionKey(encryptionKey);
        }

        [TestMethod]
        public void EncryptAndDecryptValidInputsSuccess()
        {
            string originalData = Faker.Address.StreetAddress();
            string encryptedData = SecureDataEncryptionHelper.Encrypt(originalData);
            string decryptedData = SecureDataEncryptionHelper.Decrypt(encryptedData);
            Assert.AreNotEqual(originalData, encryptedData);
            Assert.AreEqual(originalData, decryptedData);
        }

        [TestMethod]
        public void EncryptEmptyInputReturnsEmptyString()
        {
            string emptyData = string.Empty;
            string encryptedData = SecureDataEncryptionHelper.Encrypt(emptyData);
            Assert.AreEqual(string.Empty, encryptedData);
        }

        [TestMethod]
        public void DecryptInvalidInputReturnsEmptyString()
        {
            string invalidData = string.Empty;
            string decryptedData = SecureDataEncryptionHelper.Decrypt(invalidData);
            Assert.AreEqual(string.Empty, decryptedData);
        }



        [TestMethod]
        public void EmptyKeyThrowsArgumentException()
        {
            ArgumentException exception = Assert.ThrowsException<ArgumentException>(() => SecureDataEncryptionHelper.SetEncryptionKey(string.Empty));
            Assert.AreEqual("Encryption key cannot be null or empty.", exception.Message);
        }
    }
}
