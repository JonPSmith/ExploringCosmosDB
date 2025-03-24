// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SqlDataLayer.Classes;

namespace SqlDataLayer.SqlBookEfCore;

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
        //Other way show https://stackoverflow.com/questions/67589195/many-to-many-relationship-with-the-join-table-that-has-extra-data
        modelBuilder.Entity<BookAuthor>()
            .HasKey(x => new { x.BookId, x.AuthorId });

        modelBuilder.Entity<Book>().HasIndex(x => x.PublishedOn);
        modelBuilder.Entity<Book>().HasIndex(x => x.ActualPrice);

        modelBuilder.Entity<Book>().Property(x => x.OrgPrice).HasPrecision(9, 2);
        modelBuilder.Entity<Book>().Property(x => x.ActualPrice).HasPrecision(9, 2);
    }
}