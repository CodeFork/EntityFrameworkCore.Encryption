using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Encryption.TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.Configure<EncryptionOptions>(opt =>
            {
                opt.Key = "Ha7d31+5tnLm7QrWyEBis7iXb7bcMdzFGp6DSltH+RI=";
                opt.InitializationVector = "P6pYi+oyPqpfZxfdYkwAWQ==";
            });

            services.AddOptions();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddDbContext<BloggingContext>();

            var context = services.BuildServiceProvider().GetRequiredService<BloggingContext>();

            context.Database.Migrate();
            
            /* context.Blogs.Add(new Blog
            {
                Url = "https://google.de", BlogId = 2, Rating = 2, Posts = new[]
                {
                    new Post
                    {
                        Title = "Hello World",
                        Content = "This is only a test",
                        PostId = 2
                    }
                }.ToList()
            });
            
            
            context.SaveChanges(); */

            var posts = context.Posts.ToList();

            foreach (var post in posts)
            {
                Console.WriteLine($"Post {post.PostId} {post.Title} {post.Content}");
            }
        }
    }
}