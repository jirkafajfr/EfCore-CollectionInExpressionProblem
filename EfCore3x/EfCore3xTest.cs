using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EfCore3x
{
    public class EfCore3xTest : IDisposable
    {
        private readonly BloggingContext _context;

        public EfCore3xTest(ITestOutputHelper output)
        {
            // Create logger
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(output));

            // Setup database and apply migrations
            _context = new BloggingContext(loggerFactory);
            _context.Database.OpenConnection();
            _context.Database.Migrate();

            // Add sample record
            var blog = _context.Add(new Blog 
            {
                Url = "http://blogs.msdn.com/adonet"
            });
            blog.Entity.Posts.Add(new Post
            {
                Title = "Hello World",
                Content = "I wrote an app using EF Core!"
            });

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        /// <summary>
        /// Type of `ids` variable is `List<T>`. EfCore 3.x translates query as:
        /// 
        /// `
        /// SELECT "post"."BlogId", "post"."Url"
        /// FROM "Blogs" AS "post"
        /// WHERE "post"."BlogId" IN(1, 2)
        /// `
        /// </summary>
        [Fact]
        public void ListUsedInExpressionTest()
        {
            var ids = new List<int>
            {
                1,
                2
            };

            _context.Blogs
                .Where(post => ids.Contains(post.BlogId))
                .Should()
                .NotBeEmpty();
        }

        /// <summary>
        /// Type of `ids` variable is `ICollection<T>`. EfCore 3.x crashes on
        ///
        /// `
        /// System.InvalidOperationException : The LINQ expression 'Where<Blog>(
        /// source: DbSet<Blog>,
        /// predicate: (b) => (Unhandled parameter: __ids_0).Contains(b.BlogId))' could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to either AsEnumerable(), AsAsyncEnumerable(), ToList(), or ToListAsync(). See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.
        /// `
        ///
        /// It sounds like a bug. Following query should be translated to the same
        /// select as the List version.
        /// </summary>
        [Fact]
        public void CollectionUsedInExpressionTest()
        {
            ICollection<int> ids = new List<int>
            {
                1,
                2
            };

            _context.Blogs
                .Where(post => ids.Contains(post.BlogId))
                .Should()
                .NotBeEmpty();
        }
    }
}
