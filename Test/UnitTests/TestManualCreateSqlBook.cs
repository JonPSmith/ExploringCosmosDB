// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SqlDataLayer;
using SqlDataLayer.Classes;
using SqlDataLayer.SqlBookEfCore;
using TestSupport.EfHelpers;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;


namespace Test.UnitTests;

public class TestManualCreateSqlBook(ITestOutputHelper output)
{
    [Fact]
    public void TestCreateOneBook()
    {
        //SETUP

        //ATTEMPT
        var book = new Book
        {
            Title = "Book title",
            Authors = new List<Author>
            {
                new Author{Name = "author1"},
                new Author{Name = "author2"}
            },
            PublishedOn = new DateOnly(2025, 1, 13),
            Publisher = "Manning",
            OrgPrice = 123,
            ActualPrice = 123,
            ImageUrl = "Url to book ",
            Tags = new List<Tag> { new Tag { TagId = "My Tag" } },
            BookAuthors = new List<BookAuthor>()
        };
        byte order = 0;
        foreach (var author in book.Authors)
        {
            book.BookAuthors.Add(new BookAuthor { Book = book, Author = author, Order = order });
            order++;
        }

        //VERIFY
        output.WriteLine(book.ToString());
        book.ToString().ShouldEqual(
            "Book title by author1, author2. Price 123, no reviews, Published by Manning on 13/01/2025, Tags: My Tag");
    }

    [Fact]
    public void TestCreateOneBook_ToDatabase()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookSqlDbContext>();
        using var context = new BookSqlDbContext(options);
        context.Database.EnsureClean();

        var book = new Book
        {
            Title = "Book title",
            Authors = new List<Author>
            {
                new Author{Name = "author1"},
                new Author{Name = "author2"}
            },
            PublishedOn = new DateOnly(2025, 1, 13),
            Publisher = "Manning",
            OrgPrice = 123,
            ActualPrice = 123,
            ImageUrl = "Url to book ",
            Tags = new List<Tag> { new Tag { TagId = "My Tag" } },
            BookAuthors = new List<BookAuthor>()
        };
        byte order = 0;
        foreach (var author in book.Authors)
        {
            book.BookAuthors.Add(new BookAuthor { Book = book, Author = author, Order = order });
            order++;
        }

        //ATTEMPT
        context.Add(book);
        context.SaveChanges();

        //VERIFY
        context.ChangeTracker.Clear();
        output.WriteLine(book.ToString());
        var bookdb = context.Books
            .Include(x => x.Authors)
                .ThenInclude(y => y.BookAuthors)
            .Include(x => x.Reviews)
            .Include(x => x.Tags)
            .Include(x => x.Promotion)
            .FirstOrDefault();
        bookdb.ShouldNotBeNull();
        bookdb.BookAuthors.Count.ShouldEqual(2);
        bookdb.Reviews.Count.ShouldEqual(0);
        bookdb.Tags.Count.ShouldEqual(1);
        bookdb.Promotion.ShouldBeNull();

        book.ToString().ShouldEqual(
            "Book title by author1, author2. Price 123, no reviews, Published by Manning on 13/01/2025, Tags: My Tag");
    }
}