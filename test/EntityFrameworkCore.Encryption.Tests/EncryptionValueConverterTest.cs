using System;
using Moq;
using Xunit;

namespace EntityFrameworkCore.Encryption.Tests
{
    public class EncryptionValueConverterTest
    {
        private readonly Mock<IEncryptionService> _encryptionService;

        public EncryptionValueConverterTest()
        {
            _encryptionService = new Mock<IEncryptionService>();
            _encryptionService.Setup(x => x.Encrypt(It.IsAny<object>())).Returns("ENCRYPTED_VALUE");
        }

        [Theory]
        [InlineData('c')]
        [InlineData("Hello World")]
        [InlineData(47)]
        [InlineData(47.02)]
        [InlineData(true)]
        public void ConvertTo_Should_Pass_Param_To_EncryptionService_And_Wrap_Returned_Value(object val)
        {
            var returnVal = EncryptionValueConverter<object>.ConvertTo(_encryptionService.Object, val);

            _encryptionService.Verify(x => x.Encrypt(It.Is<object>(y => y == val)));
            Assert.Equal("ENC-ENCRYPTED_VALUE", returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Not_Encrypted_Just_Return_It_String()
        {
            const string unEncryptedString = "MY_UNENCRYPTED_STRING";
            var returnVal = EncryptionValueConverter<string>.ConvertFrom(_encryptionService.Object, unEncryptedString);

            _encryptionService.Verify(x => x.Decrypt<string>(It.IsAny<string>()), Times.Never);
            Assert.Equal(unEncryptedString, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Not_Encrypted_Just_Return_It_Float()
        {
            const double myFloat = 47.123;
            var returnVal = EncryptionValueConverter<double>.ConvertFrom(_encryptionService.Object, $"{myFloat}");

            _encryptionService.Verify(x => x.Decrypt<double>(It.IsAny<string>()), Times.Never);
            Assert.Equal(myFloat, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Not_Encrypted_Just_Return_It_Int()
        {
            const int myInt = 47;
            var returnVal = EncryptionValueConverter<int>.ConvertFrom(_encryptionService.Object, $"{myInt}");

            _encryptionService.Verify(x => x.Decrypt<int>(It.IsAny<string>()), Times.Never);
            Assert.Equal(myInt, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Not_Encrypted_Just_Return_It_Bool()
        {
            const bool myBool = true;
            var returnVal = EncryptionValueConverter<bool>.ConvertFrom(_encryptionService.Object, $"{myBool}");

            _encryptionService.Verify(x => x.Decrypt<bool>(It.IsAny<string>()), Times.Never);
            Assert.Equal(myBool, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Is_Encrypted_Return_Decrypted_String()
        {
            const string unEncryptedString = "MY_UNENCRYPTED_STRING";
            _encryptionService.Setup(x => x.Decrypt<string>(It.IsAny<string>())).Returns(unEncryptedString);

            var returnVal =
                EncryptionValueConverter<string>.ConvertFrom(_encryptionService.Object, $"ENC-{unEncryptedString}");

            _encryptionService.Verify(x => x.Decrypt<string>(It.Is<string>(y => y == unEncryptedString)));
            Assert.Equal(unEncryptedString, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Is_Encrypted_Return_Decrypted_String_Handle_Empty_String()
        {
            var unEncryptedString = string.Empty;
            _encryptionService.Setup(x => x.Decrypt<string>(It.IsAny<string>())).Returns(unEncryptedString);

            var returnVal =
                EncryptionValueConverter<string>.ConvertFrom(_encryptionService.Object, $"ENC-{unEncryptedString}");

            _encryptionService.Verify(x => x.Decrypt<string>(It.Is<string>(y => y == unEncryptedString)));
            Assert.Equal(unEncryptedString, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Is_Encrypted_Return_Decrypted_Float()
        {
            const double myFloat = 47.123;
            _encryptionService.Setup(x => x.Decrypt<double>(It.IsAny<string>())).Returns(myFloat);

            var returnVal = EncryptionValueConverter<double>.ConvertFrom(_encryptionService.Object, $"ENC-{myFloat}");

            _encryptionService.Verify(x => x.Decrypt<double>(It.Is<string>(y => y == $"{myFloat}")));
            Assert.Equal(myFloat, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Is_Encrypted_Return_Decrypted_Int()
        {
            const int myInt = 47;
            _encryptionService.Setup(x => x.Decrypt<int>(It.IsAny<string>())).Returns(myInt);

            var returnVal = EncryptionValueConverter<int>.ConvertFrom(_encryptionService.Object, $"ENC-{myInt}");

            _encryptionService.Verify(x => x.Decrypt<int>(It.Is<string>(y => y == $"{myInt}")));
            Assert.Equal(myInt, returnVal);
        }

        [Fact]
        public void ConvertFrom_Value_Is_Encrypted_Return_Decrypted_Bool()
        {
            const bool myBool = true;
            _encryptionService.Setup(x => x.Decrypt<bool>(It.IsAny<string>())).Returns(myBool);

            var returnVal = EncryptionValueConverter<bool>.ConvertFrom(_encryptionService.Object, $"ENC-{myBool}");

            _encryptionService.Verify(x => x.Decrypt<bool>(It.Is<string>(y => y == $"{myBool}")));
            Assert.Equal(myBool, returnVal);
        }
    }
}