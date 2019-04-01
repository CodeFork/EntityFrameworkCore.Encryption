using System;
using System.Runtime.Serialization;

namespace EntityFrameworkCore.Encryption.Exceptions
{
    public class DecryptionException : InvalidOperationException
    {
        public DecryptionException()
        {
        }

        protected DecryptionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DecryptionException(string message) : base(message)
        {
        }

        public DecryptionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}