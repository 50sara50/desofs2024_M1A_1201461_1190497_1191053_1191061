using StreamingPlatform.Utils;

namespace StreamingService.Test.Utils
{
    [TestClass]
    public class PasswordEncryptorTests
    {
        private const string TestPassword = "P@ssw0rd";
        private const string TestPepper = "SecretPepper";

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
            int iteration = 50;
            string encryptedPassword1 = PasswordEncryptor.EncryptPassword(TestPassword, salt, TestPepper, iteration);
            string encryptedPassword2 = PasswordEncryptor.EncryptPassword(TestPassword, salt, TestPepper, iteration);
            Assert.AreEqual(encryptedPassword1, encryptedPassword2);
        }
    }

}
