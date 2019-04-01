using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.Encryption
{
    public class EncryptionValueConverter<TModel> : ValueConverter<TModel, string>
    {
        private const string EncryptionPrefix = "ENC-";

        public EncryptionValueConverter(IEncryptionService encryptionService,
            ConverterMappingHints mappingHints = default(ConverterMappingHints)) : base(
            model => ConvertTo(encryptionService, model),
            encryptedString => ConvertFrom(encryptionService, encryptedString),
            mappingHints)
        {
        }

        /// <summary>
        /// ConvertTo function that is used internally to convert the value of a property to its encrypted representation.
        /// Warning: this function is used internally and only public as it's tested by the unit test project.
        /// </summary>
        /// <param name="service">the encryption service</param>
        /// <param name="model">the model that should be encrypted</param>
        /// <returns>prefixed and encrypted representation of the value</returns>
        public static string ConvertTo(IEncryptionService service, TModel model)
        {
            return WrapEncryptedString(service.Encrypt(model));
        }

        /// <summary>
        /// ConvertFrom function that is used internally to convert the encrypted property value to its decrypted representation.
        /// If the value was not yet encrypted, it is simply converted and returned (determined by an encryption prefix).
        /// Warning: this function is used internally and only public as it's tested by the unit test project.
        /// </summary>
        /// <param name="service">the encryption service</param>
        /// <param name="encryptedString">the string that should be decrypted. If it's encrypted, it needs to be prefixed by the Encryption prefix.</param>
        /// <returns>decrypted value</returns>
        public static TModel ConvertFrom(IEncryptionService service, string encryptedString)
        {
            return IsEncryptedString(encryptedString)
                ? service.Decrypt<TModel>(UnwrapEncryptedString(encryptedString))
                : (TModel) Convert.ChangeType(encryptedString, typeof(TModel));
        }

        private static bool IsEncryptedString(string encryptedString)
        {
            return encryptedString?.StartsWith(EncryptionPrefix) ?? false;
        }

        private static string WrapEncryptedString(string encryptedString)
        {
            return EncryptionPrefix + encryptedString;
        }

        private static string UnwrapEncryptedString(string encryptedString)
        {
            var index = encryptedString?.IndexOf(EncryptionPrefix) ?? -1;
            return index > -1 ? encryptedString?.Substring(index + EncryptionPrefix.Length) : encryptedString;
        }
    }

    public static class EncryptionValueConverterFactory
    {
        /// <summary>
        /// Can be used to create a new instance of the value converter (used for reflection)
        /// </summary>
        /// <param name="encryptionService">the encryption service</param>
        /// <returns></returns>
        public static EncryptionValueConverter<TModel> CreateInstance<TModel>(IEncryptionService encryptionService)
        {
            return new EncryptionValueConverter<TModel>(encryptionService);
        }
    }
}