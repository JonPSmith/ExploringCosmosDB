// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.Classes;

namespace SqlDataLayer.SqlBookEfCore
{
    public class BookSqlDbContext : DbContext
    {
        public BookSqlDbContext(DbContextOptions<BookSqlDbContext> options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }                        
        public DbSet<BookDetails> Details { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

