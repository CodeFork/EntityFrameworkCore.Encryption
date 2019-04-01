using EntityFrameworkCore.Encryption.Attributes;

namespace EntityFrameworkCore.Encryption.TestProject
{
    [Encrypt]
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        [ExcludeFromEncryption] public int BlogId { get; set; }
        public Blog Blog { get; set; }

        [ExcludeFromEncryption] public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}