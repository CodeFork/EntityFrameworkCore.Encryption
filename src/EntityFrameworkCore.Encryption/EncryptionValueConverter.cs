using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.Encryption
{
    public class EncryptionValueConverter<TModel> : ValueConverter<TModel, string>
    {
        public EncryptionValueConverter(IEncryptionService encryptionService,
            ConverterMappingHints mappingHints = default(ConverterMappingHints)) : base(
            (TModel model) => encryptionService.Encrypt(model),
            (string encryptedString) => encryptionService.Decrypt<TModel>(encryptedString),
            mappingHints)
        {
        }
    }
}