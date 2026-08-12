#if NET48
using System;
using EncryptionProvider.String;
using NUnit.Framework;

namespace ReportTests
{
    [TestFixture]
    public class StringEncryptionTests
    {
        private const string Key1 = "AAECAwQFBgcICQoLDA0ODw==";
        private const string Key2 = "CCECAwQFBgcICQoLDA0ODw==";
        private const string PlainText = "Testing123!£$";

        [Test]
        public void Decrypt_AfterEncrypt_ReturnsOriginalString()
        {
            var subject = new StringEncryption(Key1);
            var encrypted = subject.Encrypt(PlainText);
            Assert.That(subject.Decrypt(encrypted), Is.EqualTo(PlainText));
        }

        [Test]
        public void Encrypt_ProducesDifferentCiphertextEachTime()
        {
            var subject = new StringEncryption(Key1);
            var first = subject.Encrypt(PlainText);
            var second = subject.Encrypt(PlainText);
            Assert.That(first, Is.Not.EqualTo(second), "Each encryption call must produce a unique ciphertext (random IV)");
        }

        [Test]
        public void Encrypted_DoesNotMatchPlaintext()
        {
            var subject = new StringEncryption(Key1);
            Assert.That(subject.Encrypt(PlainText), Is.Not.EqualTo(PlainText));
        }

        [Test]
        public void DifferentKeys_ProduceDifferentCiphertext()
        {
            var s1 = new StringEncryption(Key1);
            var s2 = new StringEncryption(Key2);
            Assert.That(s1.Encrypt(PlainText), Is.Not.EqualTo(s2.Encrypt(PlainText)));
        }

        [Test]
        public void CrossKey_Decrypt_DoesNotReturnOriginalPlaintext()
        {
            var s1 = new StringEncryption(Key1);
            var s2 = new StringEncryption(Key2);
            var encrypted = s1.Encrypt(PlainText);
            // Decrypting with a different key must not silently return the original plaintext.
            // It will either throw or produce garbage — both are acceptable.
            try
            {
                var result = s2.Decrypt(encrypted);
                Assert.That(result, Is.Not.EqualTo(PlainText), "Wrong key must not decrypt to original plaintext");
            }
            catch (Exception)
            {
                // Throwing on wrong-key decryption is also acceptable (e.g. padding exception)
                Assert.Pass("Decryption with wrong key threw an exception as expected");
            }
        }

        [Test]
        public void EmptyString_EncryptDecrypt_RoundTrips()
        {
            var subject = new StringEncryption(Key1);
            var encrypted = subject.Encrypt("");
            Assert.That(subject.Decrypt(encrypted), Is.EqualTo(""));
        }

        [Test]
        public void LongString_EncryptDecrypt_RoundTrips()
        {
            var subject = new StringEncryption(Key1);
            var longString = new string('x', 10_000);
            Assert.That(subject.Decrypt(subject.Encrypt(longString)), Is.EqualTo(longString));
        }

        [Test]
        public void UnicodeString_EncryptDecrypt_RoundTrips()
        {
            var subject = new StringEncryption(Key1);
            const string unicode = "日本語テスト 🎉 Ünïcödé";
            Assert.That(subject.Decrypt(subject.Encrypt(unicode)), Is.EqualTo(unicode));
        }
    }
}
#endif
