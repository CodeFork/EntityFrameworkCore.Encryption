using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        private readonly EncryptionOptions _options;

        public EncryptionService(IOptions<EncryptionOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc />
        public string Encrypt<T>(T obj)
        {
            var buffer = ObjectToByteArray(obj);
            var byteResult = PerformCryptoAlgorithm(buffer, aes => aes.CreateEncryptor(aes.Key, aes.IV));

            return Convert.ToBase64String(byteResult);
        }

        /// <inheritdoc />
        public T Decrypt<T>(string obj)
        {
            try
            {
                var data = Convert.FromBase64String(obj);
                var byteResult = PerformCryptoAlgorithm(data, aes => aes.CreateDecryptor(aes.Key, aes.IV));

                return (T) ByteArrayToObject(byteResult);
            }
            catch (Exception ex)
            {
                throw new DecryptionException("Error decrypting data. See innerException for details", ex);
            }
        }

        /// Acknowledgements: Implementation was inspired by: https://stackoverflow.com/a/41344959 (THANKS)
        private byte[] PerformCryptoAlgorithm(byte[] data, Func<Aes, ICryptoTransform> createTransformFunc)
        {
            byte[] result;

            using (var aes = Aes.Create())
            {
                if (aes == null)
                {
                    throw new InvalidOperationException("Excepted valid aes instance");
                }

                aes.Key = Convert.FromBase64String(_options.Key);
                aes.IV = Convert.FromBase64String(_options.InitializationVector);

                using (var transform = createTransformFunc(aes))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, transform, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(data))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    result = resultStream.ToArray();
                }
            }

            return result;
        }

        /// Taken from: https://stackoverflow.com/a/10502856
        private static byte[] ObjectToByteArray(object obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// Taken from: https://stackoverflow.com/a/10502856
        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}