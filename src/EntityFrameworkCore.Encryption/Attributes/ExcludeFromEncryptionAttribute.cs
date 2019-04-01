using System;

namespace EntityFrameworkCore.Encryption.Attributes
{
    /// <summary>
    /// If you apply this flag to a property of an encrypted entity, it will not be encrypted.
    /// Warning: If the fields have already been encrypted, you'll have to decrypt the fields manually.
    /// </summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeFromEncryptionAttribute : Attribute
    {
        
    }
}