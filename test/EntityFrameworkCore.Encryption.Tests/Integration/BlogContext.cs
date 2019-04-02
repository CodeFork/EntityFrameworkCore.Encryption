using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Encryption.Tests.Integration
{
    public class BlogContext : DbContext
    {
        private readonly IEncryptionService _service;

        public DbSet<Blog> Blogs { get; set; }

        public BlogContext(IEncryptionService service)
        {
            _service = service;
        }

        public BlogContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // use sqlite
            optionsBuilder.UseSqlite("Data Source=blogging.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (Environment.GetEnvironmentVariable("UseEncryption") == true.ToString().ToLower())
            {
                modelBuilder.ApplyEncryption(_service);
            }
        }
    }
}