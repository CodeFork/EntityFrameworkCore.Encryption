using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Encryption
{
    /// <summary>
    /// This service can be used to encrypt all data in the database.
    /// Note: It should not be run in the usual startup of your application as it iterates (and loads) over all
    /// entities of your database and marks them as modified.
    /// </summary>
    public interface IEncryptionMigrator
    {
        /// <summary>
        /// Ensures that all entities of the database of the context that should be encrypted (that have the [Encrypt] attribute)
        /// become encrypted.
        /// </summary>
        /// <param name="context">the context that specifies the database for encryption</param>
        void EncryptDatabase(DbContext context);
    }
}