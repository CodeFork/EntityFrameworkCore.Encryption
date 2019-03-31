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
            // use sqlite
            // optionsBuilder.UseSqlite("Data Source=blogging.db");

            optionsBuilder.UseNpgsql(
                "User ID=admin;Password=Test1234;Host=localhost;Port=25432;Database=encryption-blogging-db;Pooling=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>().Property(x => x.Title)
                .HasConversion(new EncryptionValueConverter<string>(_service));

            modelBuilder.Entity<Post>().Property(x => x.Content)
                .HasConversion(new EncryptionValueConverter<string>(_service));

            modelBuilder.Entity<Blog>().Property(x => x.Rating)
                .HasConversion(new EncryptionValueConverter<int>(_service));
        }
    }
}