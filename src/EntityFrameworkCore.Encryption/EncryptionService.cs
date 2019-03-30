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
        /// Acknowledgements: Implementation was inspired by: https://stackoverflow.com/a/41344959 (THANKS)
        public string Encrypt<T>(T obj)
        {
            byte[] result;
            var buffer = ObjectToByteArray(obj);

            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_options.Key);
                aes.IV = Convert.FromBase64String(_options.InitializationVector);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(buffer))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    result = resultStream.ToArray();
                }
            }

            return Convert.ToBase64String(result);
        }

        /// <inheritdoc />
        /// Acknowledgements: Implementation was inspired by: https://stackoverflow.com/a/41344959 (THANKS)
        public T Decrypt<T>(string obj)
        {
            T result;
            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_options.Key);
                aes.IV = Convert.FromBase64String(_options.InitializationVector);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(Convert.FromBase64String(obj)))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    result = (T) ByteArrayToObject(resultStream.ToArray());
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