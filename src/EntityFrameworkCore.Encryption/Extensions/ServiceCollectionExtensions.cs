using System;
using EntityFrameworkCore.Encryption;
using EntityFrameworkCore.Encryption.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the encryption by adding the options passed as an argument
        /// and also necessary services to the given service collection.
        /// </summary>
        /// <param name="services">the service collection</param>
        /// <param name="encryptionOptions">the encryption options, if null an ArgumentNullException will be thrown</param>
        /// <returns>extended service collection</returns>
        public static IServiceCollection AddDatabaseEncryption(this IServiceCollection services,
            EncryptionOptions encryptionOptions)
        {
            if (encryptionOptions == null)
            {
                throw new ArgumentNullException();
            }

            services.Configure<EncryptionOptions>(opt =>
            {
                opt.Key = encryptionOptions.Key;
                opt.InitializationVector = encryptionOptions.InitializationVector;
            });

            services.AddOptions();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<IEncryptionMigrator, EncryptionMigrator>();

            return services;
        }
    }
}