using EntityFrameworkCore.Encryption.Attributes;

namespace EntityFrameworkCore.Encryption.Tests.Integration
{
    [Encrypt]
    public class Blog
    {
        [ExcludeFromEncryption] public int BlogId { get; set; }
        public string Name { get; set; }
    }
}