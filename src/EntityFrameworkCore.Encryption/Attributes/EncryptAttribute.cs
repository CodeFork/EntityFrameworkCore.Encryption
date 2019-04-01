using System;

namespace EntityFrameworkCore.Encryption.Attributes
{
    /// <summary>
    /// If you apply this flag to an entity of a DbContext, this entity will be automatically encrypted.
    /// That means that all backing fields of properties in the database will be altered to a string that contains
    /// the specific value. The mapping back and forth happens transparently, so you won't have to worry about that.
    /// If you want to exclude certain properties from encryption, you can apply the <see cref="ExcludeFromEncryptionAttribute"/>
    /// [ExcludeFromEncryptionAttribute].
    /// </summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class)]
    public class EncryptAttribute : Attribute
    {
        
    }
}