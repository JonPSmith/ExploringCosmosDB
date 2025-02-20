// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Xunit.Abstractions;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests;

public class TestSqlCreateBooksFromManningData(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Theory]
    [InlineData(10)]
    [InlineData(1000)]
    public void TestCreateSqlManningBooks_Count(int numBooks)
    {
        //SETUP
        var creator = new CreateSqlBooksFromManningData();

        //ATTEMPT
        var manningBooks = creator.CreateSqlManningBooks(numBooks).ToArray();

        //VERIFY
        manningBooks.Length.ShouldEqual(numBooks);
        _output.WriteLine( $"Num: {numBooks}, Good years: {manningBooks.Count(x => x.PublishedOn > new DateOnly(1000,1,1))}");
        ;
    }

    [Fact]
    public void TestCreateSqlManningBooks_CheckCopy()
    {
        //SETUP
        var creator = new CreateSqlBooksFromManningData();

        //ATTEMPT
        var manningBooks = creator.CreateSqlManningBooks(762+1).ToArray();

        //VERIFY
        manningBooks[0].Title.ShouldEndWith("(copy 0)");
        manningBooks[762].Title.ShouldEndWith("(copy 1)");
    }

    [Fact]
    public void TestCreateSqlManningBooks_Reviews()
    {
        //SETUP
        var creator = new CreateSqlBooksFromManningData();

        //ATTEMPT
        var manningBooks = creator.CreateSqlManningBooks(14).ToArray();

        //VERIFY
        foreach (var t in manningBooks)
        {
            //string.Format("{0}", i)
            _output.WriteLine($"NumReviews {t.ReviewsCount:00}, Average: {t.ReviewsAverageVotesCache:#.###}");
        }
    }

    [Fact]
    public void TestCreateSqlManningBooks_ToDatabase()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookSqlDbContext>();
        using var context = new BookSqlDbContext(options);
        context.Database.EnsureClean();
        var creator = new CreateSqlBooksFromManningData();

        var manningBooks = creator.CreateSqlManningBooks(14).ToArray();

        //ATTEMPT
        context.AddRange(manningBooks);
        context.SaveChanges();

        //VERIFY
        _output.WriteLine($"Num books: {context.Books.Count()}");
        _output.WriteLine($"Num Authors: {context.Authors.Count()}");
        _output.WriteLine($"Num Tags: {context.Tags.Count()}");

        context.ChangeTracker.Clear();
        var books = context.Books
            .Include(x => x.Authors)
            .ThenInclude(y => y.BookAuthors)
            .Include(x => x.Reviews)
            .Include(x => x.Tags)
            .Include(x => x.Promotion)
            .ToList();
        foreach (var book in books)
        {
            _output.WriteLine(book.ToString());
        }

    }

}