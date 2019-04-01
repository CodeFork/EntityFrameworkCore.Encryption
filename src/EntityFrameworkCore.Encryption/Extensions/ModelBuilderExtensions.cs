using System;
using System.Linq;
using System.Reflection;
using EntityFrameworkCore.Encryption;
using EntityFrameworkCore.Encryption.Attributes;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Use this method to automatically encrypt all entities that have been tagged with the [Encrypt] attribute.
        /// Warning: This method alters the column type of all encrypted columns to text!
        /// You'll still have to encrypt any existing data yourself, but you can use the <see cref="IEncryptionMigrator"/>
        /// for this.
        /// </summary>
        /// <param name="modelBuilder">the model builder</param>
        /// <param name="encryptionService">the encryption service</param>
        public static void ApplyEncryption(this ModelBuilder modelBuilder, IEncryptionService encryptionService)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().Where(x =>
                x.ClrType?.GetCustomAttribute(typeof(EncryptAttribute)) != null);

            foreach (var entityType in entityTypes)
            {
                var scalarProperties = entityType.GetProperties().Where(x =>
                    x.PropertyInfo != null &&
                    !Attribute.IsDefined(x.PropertyInfo, typeof(ExcludeFromEncryptionAttribute)));

                foreach (var property in scalarProperties)
                {
                    modelBuilder.Entity(entityType.ClrType).Property(property.Name)
                        .HasConversion(CreateValueConverter(property.ClrType, encryptionService));
                }
            }
        }

        private static dynamic CreateValueConverter(Type propertyType, IEncryptionService encryptionService)
        {
            var method =
                typeof(EncryptionValueConverterFactory).GetMethod(
                    nameof(EncryptionValueConverterFactory.CreateInstance));

            if (method == null)
            {
                throw new InvalidOperationException("Did not find factory method!");
            }

            var genRef = method.MakeGenericMethod(propertyType);

            return genRef.Invoke(null, new object[] {encryptionService});
        }
    }
}