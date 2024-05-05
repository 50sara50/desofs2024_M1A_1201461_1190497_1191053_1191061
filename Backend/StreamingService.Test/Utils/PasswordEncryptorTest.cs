using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using StreamingPlatform.Dao;
using StreamingPlatform.Utils;
using System.Reflection;

namespace StreamingService.Test.Utils
{
    [TestClass]
    public class PasswordEncryptorTests
    {
        private const string TestPassword = "P@ssw0rd";
        private string TestPepper = "SecretPepper";
        private int TestIterations = 100;

        [TestInitialize]

        public void Initialize()
        {
            // the type specified here is just so the secrets library can 
            // find the UserSecretId we added in the csproj file
            IConfigurationBuilder secretsBuilder = new ConfigurationBuilder()
                .AddUserSecrets<StreamingDbContext>();
            IConfigurationRoot secretsConfiguration = secretsBuilder.Build();
            string? passwordPepper = secretsConfiguration.GetValue<string>("Keys:PasswordPepper");
            if (passwordPepper != null)
            {
                TestPepper = passwordPepper;
            }
            Dictionary<string, string?> inMemorySettings = new()
            {
                { "HashingIterations","1000"}
            };
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings);
            IConfigurationRoot appSettings = builder.Build();
            string? iterations = appSettings.GetValue<string?>("HashingIterations");
            if (iterations != null)
            {
                TestIterations = int.Parse(iterations);
            }

        }

        [TestMethod]
        public void EncryptPasswordValidInputReturnsNonEmptyString()
        {
            string salt = PasswordEncryptor.GenerateSalt();
            int iteration = 100;
            string encryptedPassword = PasswordEncryptor.EncryptPassword(TestPassword, salt, TestPepper, iteration);
            Assert.IsFalse(string.IsNullOrWhiteSpace(encryptedPassword));
        }

        [TestMethod]
        public void GenerateSaltReturnsValidSalt()
        {
            string salt = PasswordEncryptor.GenerateSalt();
            Assert.IsFalse(string.IsNullOrWhiteSpace(salt));
            Assert.IsTrue(salt.Length > 0);
        }

        [TestMethod]
        public void EncryptPasswordWithZeroIterationsReturnsOriginalPassword()
        {
            string salt = PasswordEncryptor.GenerateSalt();
            int iteration = 0;
            string encryptedPassword = PasswordEncryptor.EncryptPassword(TestPassword, salt, TestPepper, iteration);
            Assert.AreEqual(TestPassword, encryptedPassword);
        }

        [TestMethod]
        public void EncryptPasswordSameInputAndParametersReturnsSameOutput()
        {
            string salt = PasswordEncryptor.GenerateSalt();
            string encryptedPassword1 = PasswordEncryptor.EncryptPassword(TestPassword, salt, TestPepper, TestIterations);
            string encryptedPassword2 = PasswordEncryptor.EncryptPassword(TestPassword, salt, TestPepper, TestIterations);
            Assert.AreEqual(encryptedPassword1, encryptedPassword2);
        }
    }

}
