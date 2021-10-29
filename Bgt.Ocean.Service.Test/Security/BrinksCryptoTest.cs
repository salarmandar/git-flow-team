using System;
using System.Collections.Generic;
using Bgt.Ocean.Infrastructure.Security;
using Xunit;

namespace Bgt.Ocean.Service.Test.Security
{
    public class BrinksCryptoTest : BaseTest
    {
        public static IEnumerable<object[]> MockChipherData
        {
            get
            {
                return new List<string[]>
                {
                    new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                    new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString()  },
                    new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
                };
            }
        }

        [Theory]
        [MemberData(nameof(MockChipherData))]
        public void CheckIfCanEncrypted(string value, string key)
        {
            var encrypted = value.AES_Encrypt(key);
            Assert.True(encrypted.IsSuccess);
        }

        [Theory]
        [MemberData(nameof(MockChipherData))]
        public void CheckIfCanDecryptProcess(string value, string key)
        {
            var encrypted = value.AES_Encrypt(key);
            var decrypted = encrypted.Message.AES_Decrypt(key);
            Assert.True(decrypted.IsSuccess);
        }

        [Fact]
        public void CheckIfCanDecryptProcess_WrongKey()
        {
            var value = Guid.NewGuid().ToString();
            var key1 = Guid.NewGuid().ToString();
            var key2 = Guid.NewGuid().ToString();

            var encrypted = value.AES_Encrypt(key1);
            var decrypted = encrypted.Message.AES_Decrypt(key2);
            Assert.False(decrypted.IsSuccess);
        }

        [Fact]
        public void CheckIfGetDifferentMessage_WrongKey()
        {
            var value = Guid.NewGuid().ToString();
            var key1 = Guid.NewGuid().ToString();
            var key2 = Guid.NewGuid().ToString();

            var encrypt = value.AES_Encrypt(key1);
            var decript = encrypt.Message.AES_Decrypt(key2);

            Assert.False(value == decript.Message);
        }

        [Theory]
        [MemberData(nameof(MockChipherData))]
        public void CheckIfGetCorrectMessage(string value, string key)
        {
            var encrypted = value.AES_Encrypt(key);
            var decrypted = encrypted.Message.AES_Decrypt(key);
            Assert.Equal(value, decrypted.Message);
        }
    }
}
