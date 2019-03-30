using System;
using Microsoft.Extensions.Options;
using Xunit;

namespace EntityFrameworkCore.Encryption.Tests
{
    public class EncryptionServiceTest
    {
        private EncryptionOptions _options;
        private IEncryptionService _service;
        
        public EncryptionServiceTest()
        {
            _options = new EncryptionOptions
            {
                Key = "Ha7d31+5tnLm7QrWyEBis7iXb7bcMdzFGp6DSltH+RI=",
                InitializationVector = "P6pYi+oyPqpfZxfdYkwAWQ==" 
            };   
            
            _service = new EncryptionService(new OptionsWrapper<EncryptionOptions>(_options));
        }
        
        [Fact]
        public void Can_Encrypt_String()
        {
            var myString = Guid.NewGuid().ToString();
            var encryptedString = _service.Encrypt(myString);
            
            Assert.NotNull(encryptedString);
            Assert.NotEqual(0, encryptedString.Length);
            Assert.NotEqual(myString, encryptedString);
        }
        
        [Fact]
        public void Can_Decrypt_Encrypted_String()
        {
            var myString = Guid.NewGuid().ToString();
            var encryptedString = _service.Encrypt(myString);
            var decryptedString = _service.Decrypt<string>(encryptedString);
            
            Assert.NotNull(decryptedString);
            Assert.NotEqual(0, encryptedString.Length);
            Assert.NotEqual(encryptedString, decryptedString);
            Assert.Equal(myString, decryptedString);
        }
        
        [Theory]
        [InlineData(47)]
        [InlineData(47.0d)]
        [InlineData(47.0f)]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData('c')]
        public void Can_Encrypt_Other_Values(object val)
        {
            var encryptedString = _service.Encrypt(val);
            
            Assert.NotNull(encryptedString);
            Assert.NotEqual(0, encryptedString.Length);
            Assert.NotEqual(val, encryptedString);
        }
        
        [Theory]
        [InlineData(47)]
        [InlineData(47.0d)]
        [InlineData(47.0f)]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData('c')]
        public void Can_Decrypt_Encrypted_Values(object val)
        {
            var encryptedString = _service.Encrypt(val);
            var decryptedValue = _service.Decrypt<object>(encryptedString);
            
            Assert.NotNull(decryptedValue);
            Assert.NotEqual(0, encryptedString.Length);
            Assert.NotEqual(encryptedString, decryptedValue);
            Assert.Equal(val, decryptedValue);
        }
    }
}
