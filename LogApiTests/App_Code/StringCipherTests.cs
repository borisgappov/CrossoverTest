using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncryptStringSample.Tests
{
    [TestClass()]
    public class StringCipherTests
    {
        [TestMethod()]
        public void EncryptTest()
        {
            var testString = "test string";
            var encrypted = StringCipher.Encrypt(testString);
            Assert.IsTrue(testString != encrypted);
        }

        [TestMethod()]
        public void DecryptTest()
        {
            var testString = "test string";
            var encrypted = StringCipher.Encrypt(testString);
            Assert.IsTrue(StringCipher.Decrypt(encrypted) == testString);
        }
    }
}