using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Encryption.TestProject
{
    public class BloggingContext : DbContext
    {
        private readonly IEncryptionService _service;
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public BloggingContext(IEncryptionService service)
        {
            _service = service;
        }

        public BloggingContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=blogging.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var postType = modelBuilder.Model.FindEntityType(typeof(Post));
            foreach (var property in postType.GetProperties())
            {
                if (property.Name == nameof(Post.Title))
                {
                    property.SetValueConverter(new EncryptionValueConverter<string>(_service));
                }
            }
        }
    }
}