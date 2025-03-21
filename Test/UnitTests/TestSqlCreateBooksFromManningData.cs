// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Xunit.Abstractions;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;
using Newtonsoft.Json.Linq;

namespace Test.UnitTests;

public class TestSqlCreateBooksFromManningData(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void TestCreateSqlManningBooks_OneBook()
    {
        //SETUP
        var creator = new CreateSqlBooksFromManningData();

        //ATTEMPT
        var book = creator.CreateSqlManningBooks(1).First();

        //VERIFY
        _output.WriteLine(book.ToString());
        book.ToString().ShouldEqual("Graph Databases in Action by Dave Bechberger, Josh Perryman. " +
                                    "Price 49.99, no reviews, Published by Manning publications on 15/11/2020, Tags: Data");
    }

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
        numBooks.ShouldEqual(manningBooks.First().Title.EndsWith("(copy 0)") ? 1000 : 10);
    }

    [Fact]
    public void TestCreateSqlManningBooks_CheckCopy()
    {
        //SETUP
        var creator = new CreateSqlBooksFromManningData();
        
        //ATTEMPT
        var manningBooks = creator.CreateSqlManningBooks(creator.NumManningBooksJson + 1).ToArray();

        //VERIFY
        manningBooks[0].Title.ShouldEndWith("(copy 0)");
        manningBooks[creator.NumManningBooksJson].Title.ShouldEndWith("(copy 1)");
        manningBooks[0].Title.Substring(0, manningBooks[0].Title.Length - "(copy 0)".Length)
            .ShouldEqual(manningBooks[creator.NumManningBooksJson].Title.Substring(0, manningBooks[0].Title.Length - "(copy 0)".Length));
        _output.WriteLine($"NumManningBooksJson is {creator.NumManningBooksJson}");
    }

    [Fact]
    public void TestCreateSqlManningBooks_TagsProfile()
    {
        //SETUP
        var creator = new CreateSqlBooksFromManningData();
        var manningBooks = creator.CreateSqlManningBooks(creator.NumManningBooksJson).ToArray();
        Dictionary<string, int> tagNumDict = new Dictionary<string, int>();
        foreach (var tagName in creator.AllTagsNames)
        {
            tagNumDict.Add(tagName,0);
        }

        //ATTEMPT
        foreach (var book in manningBooks)
        {
            foreach (var tag in book.Tags)
            {
                tagNumDict[tag.TagId] += 1;
            }
        }

        //VERIFY
        
        foreach (var keyValue in tagNumDict.OrderBy(x => x.Value))
        {
            _output.WriteLine($"{keyValue.Key}: {keyValue.Value}");
        }
    }

    [Fact]
    public void TestCreateSqlManningBook_PublishYearProfile()
    {
        //SETUP
        var creator = new CreateSqlBooksFromManningData();
        var manningBooks = creator.CreateSqlManningBooks(creator.NumManningBooksJson).ToArray();
        Dictionary<int, int> publishYearDict = new Dictionary<int, int>();
        foreach (var year in manningBooks.Select(x => x.PublishedOn.Year).Distinct())
        {
            publishYearDict.Add(year, 0);
        }

        //ATTEMPT
        foreach (var book in manningBooks)
        {
            publishYearDict[book.PublishedOn.Year] += 1;
        }

        //VERIFY

        foreach (var keyValue in publishYearDict.OrderBy(x => x.Key))
        {
            _output.WriteLine($"{keyValue.Key}, {keyValue.Value}");
        }
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