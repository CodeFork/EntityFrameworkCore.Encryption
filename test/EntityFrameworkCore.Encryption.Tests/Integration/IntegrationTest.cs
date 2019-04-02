using System;
using System.IO;
using System.Linq;
using EntityFrameworkCore.Encryption.Options;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Encryption.Tests.Integration
{
    public class IntegrationTest
    {
        private const string SecondBlogName = "My Fancy Blog";
        private const string BlogName = "My Test Blog";

        private BlogContext GetContext()
        {
            var provider = PrepareServiceCollection();

            return provider.GetRequiredService<BlogContext>();
        }

        private static IServiceProvider PrepareServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddDbContext<BlogContext>();

            services.AddDatabaseEncryption(new EncryptionOptions
            {
                InitializationVector = "P6pYi+oyPqpfZxfdYkwAWQ==",
                Key = "Ha7d31+5tnLm7QrWyEBis7iXb7bcMdzFGp6DSltH+RI="
            });

            services.AddOptions();
            return services.BuildServiceProvider();
        }

        private string GetRawBlogName(int id)
        {
            var blogName = string.Empty;
            using (var connection = new SqliteConnection("" +
                                                         new SqliteConnectionStringBuilder
                                                         {
                                                             DataSource = "blogging.db"
                                                         }))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var selectCommand = connection.CreateCommand();
                    selectCommand.Transaction = transaction;
                    selectCommand.CommandText = $"SELECT Name FROM Blogs WHERE BlogId = {id} ORDER BY BlogId ASC";
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            blogName = reader.GetString(0);
                        }
                    }

                    transaction.Commit();
                }
            }

            return blogName;
        }

        [Trait("Category", "Integration")]
        [Trait("Step", "1")]
        [Fact]
        public void Can_Use_Unencrypted_Database()
        {
            var addContext = GetContext();
            addContext.Database.Migrate();
            addContext.Blogs.Add(new Blog
            {
                BlogId = 1,
                Name = "My Test Blog"
            });
            addContext.SaveChanges();

            var context = GetContext();

            Assert.NotEqual(0, context.Blogs.Count());
            Assert.Equal(BlogName, context.Blogs.First().Name);
            Assert.Equal(BlogName, GetRawBlogName(1));
        }

        [Trait("Category", "Integration")]
        [Trait("Step", "2")]
        [Fact]
        public void Can_Store_New_Data_Encrypted_But_Keep_Old_Data_Unencrypted()
        {
            var addContext = GetContext();
            addContext.Database.Migrate();
            addContext.Blogs.Add(new Blog {BlogId = 2, Name = SecondBlogName});
            addContext.SaveChanges();

            var context = GetContext();
            var encryptionService = PrepareServiceCollection().GetRequiredService<IEncryptionService>();

            Assert.Equal(2, context.Blogs.Count());
            Assert.Equal(BlogName, context.Blogs.First().Name);
            Assert.Equal(SecondBlogName, context.Blogs.ToList().ElementAt(1).Name);

            Assert.Equal(BlogName, GetRawBlogName(1));
            Assert.DoesNotContain(SecondBlogName, GetRawBlogName(2));
            Assert.Contains("ENC-", GetRawBlogName(2));
            Assert.Contains(encryptionService.Encrypt(SecondBlogName), GetRawBlogName(2));
        }

        [Trait("Category", "Integration")]
        [Trait("Step", "3")]
        [Fact]
        public void Migrator_Should_Encrypt_All_Data()
        {
            var migratorContext = GetContext();
            var encryptionService = PrepareServiceCollection().GetRequiredService<IEncryptionService>();

            var migrator = PrepareServiceCollection().GetRequiredService<IEncryptionMigrator>();
            migrator.EncryptDatabase(migratorContext);

            var context = GetContext();

            Assert.Equal(2, context.Blogs.Count());
            Assert.Equal(BlogName, context.Blogs.First().Name);
            Assert.Equal(SecondBlogName, context.Blogs.ToList().ElementAt(1).Name);

            Assert.NotEqual(BlogName, GetRawBlogName(1));
            Assert.Contains("ENC-", GetRawBlogName(1));
            Assert.DoesNotContain(BlogName, GetRawBlogName(1));
            Assert.Contains(encryptionService.Encrypt(BlogName), GetRawBlogName(1));


            Assert.DoesNotContain(SecondBlogName, GetRawBlogName(2));
            Assert.Contains("ENC-", GetRawBlogName(2));
            Assert.Contains(encryptionService.Encrypt(SecondBlogName), GetRawBlogName(2));
        }
    }
}