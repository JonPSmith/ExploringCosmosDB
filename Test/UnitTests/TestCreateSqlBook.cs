// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenerateBooks;
using SqlDataLayer.Classes;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;


namespace Test.UnitTests;

public class TestCreateSqlBook(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void TestCreateOneBook()
    {
        //SETUP

        //ATTEMPT
        var book = CreateBooks.CreateSqlBook(
            "Book title", new DateOnly(2025, 1, 13), false, 
            "Manning", 123, null, 
            new List<string> { "author1", "author2" },
            new List<Tag>{ new Tag{ TagId = "My Tag"} });

        //VERIFY
        _output.WriteLine(book.ToString());
        book.ToString().ShouldEqual(
            "Book title by author1, author2. Price 123, no reviews, Published 13/01/2025, Tags: My Tag");
    }
}