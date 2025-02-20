// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using SqlServiceLayer.Dtos;
using SqlServiceLayer.QueryObjects;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests;

public class TestSqlBookPerformance
{
    private readonly ITestOutputHelper _output;
    private readonly int _numBooks = 10000;
    private readonly bool _seeAllLogString = false;

    public TestSqlBookPerformance(ITestOutputHelper output)
    {
        _output = output;
        var options = this.CreateUniqueClassOptions<BookSqlDbContext>();
        using var context = new BookSqlDbContext(options);

        //only create the data
        if (context.Database.CanConnect())
        {
            if (context.Books.Count() != 0)
                return;
        }

        //Need to fill the database with sample data
        WipeAndFillDatabase(options);

    }

    private void WipeAndFillDatabase(DbContextOptions<BookSqlDbContext> options)
    {
        using var context = new BookSqlDbContext(options);
        context.Database.EnsureClean();
        var creator = new CreateSqlBooksFromManningData();

        var manningBooks = creator.CreateSqlManningBooks(_numBooks);
        context.AddRange(manningBooks);
        context.SaveChanges();
    }

    //Use this when you want to change the number of books you have
    [RunnableInDebugOnly] 
    public void ChangeTheDatabase()
    {
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);
        WipeAndFillDatabase(options);
    }

    [Fact]
    public void TestSqlBook_ReadOnly()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().AsSplitQuery().ToArray();

        //VERIFY
        books.Length.ShouldEqual(_numBooks);
        foreach (var log in logs)
        {
            _output.WriteLine(log?[0..30]);
        }
    }

    //--------------------------------------------------------------------
    //Order

    [Fact]
    public void TestSqlBook_OrderByPrice()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().OrderBooksBy(SortByOptions.ByPublicationDate).AsSplitQuery().ToArray();

        //VERIFY
        books.Length.ShouldEqual(_numBooks);
        foreach (var log in logs)
        {
            _output.WriteLine(_seeAllLogString ? log : log?[0..30]);
        }
    }

    [Fact]
    public void TestSqlBook_OrderReviews_NoCache()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().OrderBooksBy(SortByOptions.ByVotes).AsSplitQuery().ToArray();

        //VERIFY
        books.Length.ShouldEqual(_numBooks);
        foreach (var log in logs)
        {
            _output.WriteLine(_seeAllLogString ? log : log?[0..30]);
        }
    }

    [Fact]
    public void TestSqlBook_OrderReviews_WithCache()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().OrderBooksBy(SortByOptions.ByVotesCache).AsSplitQuery().ToArray();

        //VERIFY
        books.Length.ShouldEqual(_numBooks);
        foreach (var log in logs)
        {
            _output.WriteLine(_seeAllLogString ? log : log?[0..30]);
        }
    }

    //-----------------------------------------------------------
    //Filter

    [Theory]
    [InlineData("2005")]
    [InlineData("2015")]
    public void TestSqlBook_Filter_ByPublication(string year)
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().FilterBooksBy(FilterByOptions.ByPublicationYear, year).AsSplitQuery().ToArray();

        //VERIFY
        _output.WriteLine($"Found {books.Length} books for year {year} out of {_numBooks} books");
        foreach (var log in logs)
        {
            _output.WriteLine(_seeAllLogString ? log : log?[0..30]);
        }
        //Code to show what years are used
        // var listOfYears = context.Books
        //     .Select(x => x.PublishedOn.Year)
        //     .Distinct().Order().ToList();
        // foreach (var year in listOfYears)
        // {
        //     _output.WriteLine($"{year}");
        // }
    }

    [Fact]
    public void TestSqlBook_Filter_ByVotes()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().FilterBooksBy(FilterByOptions.ByVotes, "4").AsSplitQuery().ToArray();

        //VERIFY
        _output.WriteLine($"Found {books.Length} books with 4 or higher votes out of {_numBooks} books");
        foreach (var log in logs)
        {
            _output.WriteLine(_seeAllLogString ? log : log?[0..30]);
        }
    }

    [Fact]
    public void TestSqlBook_Filter_ByVotesWithCache()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().FilterBooksBy(FilterByOptions.ByVotesCache, "4").AsSplitQuery().ToArray();

        //VERIFY
        _output.WriteLine($"Found {books.Length} books with 4 or higher votes out of {_numBooks} books");
        foreach (var log in logs)
        {
            _output.WriteLine(_seeAllLogString ? log : log?[0..30]);
        }
    }

    [Fact]
    public void TestSqlBook_Filter_Tags()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueClassOptionsWithLogTo<BookSqlDbContext>(logs.Add);
        using var context = new BookSqlDbContext(options);

        //ATTEMPT
        BookSqlListDto[] books;
        books = context.Books.MapBookToDto().FilterBooksBy(FilterByOptions.ByTags, "C#").AsSplitQuery().ToArray();

        //VERIFY
        _output.WriteLine($"Found {books.Length} books with tag 'C#' {_numBooks} books");
        foreach (var log in logs)
        {
            _output.WriteLine(_seeAllLogString ? log : log?[0..30]);
        }
    }
}