namespace EntityFrameworkCore.Encryption
{
    /// <summary>
    /// The encryption service is the main utility for encrypting and decrypting data.
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts the object and returns it's encrypted representation
        /// </summary>
        /// <param name="obj">the object that should be encrypted</param>
        /// <typeparam name="T">the type of the object</typeparam>
        /// <returns>the encrypted representation of the object</returns>
        string Encrypt<T>(T obj);

        /// <summary>
        /// Decrypts the object and returns it's actual value
        /// </summary>
        /// <param name="obj">the object that should be decrypted</param>
        /// <typeparam name="T">the type to which the object should be casted</typeparam>
        /// <returns>the type to which the object should be casted</returns>
        T Decrypt<T>(string obj);
    }
}