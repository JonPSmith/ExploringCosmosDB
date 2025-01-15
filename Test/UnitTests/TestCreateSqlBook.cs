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

public class TestCreateSqlBook(ITestOutputHelper output)
{
    [Fact]
    public void TestCreateOneBook()
    {
        //SETUP

        //ATTEMPT
        var book = CreateSqlBooks.CreateBook(
            "Book title", new DateOnly(2025, 1, 13), 
            "Manning", 123, null, 
            new List<string> { "author1", "author2" },
            new List<Tag>{ new Tag{ TagId = "My Tag"} }, 
            new List<Review> { new Review{NumStars = 5, Comment = "Great!", VoterName = "MrBig"} });

        //VERIFY
        output.WriteLine(book.ToString());
        book.ToString().ShouldEqual(
            "Book title by author1, author2. Price 123, " +
            "1 reviews, stars = 5, Published by Manning on 13/01/2025, Tags: My Tag");
    }

    [Fact]
    public void TestCreateOneBook_ToDatabase()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookSqlDbContext>();
        using var context = new BookSqlDbContext(options);
        context.Database.EnsureClean();

        var book = CreateSqlBooks.CreateBook(
            "Book title", new DateOnly(2025, 1, 13), 
            "Manning", 123, null,
            new List<string> { "author1", "author2" },
            new List<Tag> { new Tag { TagId = "My Tag" } },
            new List<Review> { new Review { NumStars = 5, Comment = "Great!", VoterName = "MrBig" } });

        //ATTEMPT
        context.Add(book);
        context.SaveChanges();

        //VERIFY
        context.ChangeTracker.Clear();
        output.WriteLine(book.ToString());
        var bookdb = context.Books
            .Include(x => x.AuthorsLink)
                .ThenInclude(bookAuthor => bookAuthor.Author)
            .Include(x => x.Reviews)
            .Include(x => x.Tags)
            .Include(x => x.Promotion)
            .FirstOrDefault();
        bookdb.ShouldNotBeNull();
        bookdb.AuthorsLink.Count.ShouldEqual(2);
        bookdb.Reviews.Count.ShouldEqual(1);
        bookdb.Tags.Count.ShouldEqual(1);
        bookdb.Promotion.ShouldBeNull();

        book.ToString().ShouldEqual(
            "Book title by author1, author2. Price 123, " +
            "1 reviews, stars = 5, Published by Manning on 13/01/2025, Tags: My Tag");
    }
}