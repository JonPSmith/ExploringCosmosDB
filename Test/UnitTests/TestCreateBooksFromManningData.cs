// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestPlatform.Utilities;
using SqlDataLayer.Classes;
using SqlDataLayer;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using TestSupport.EfHelpers;

namespace Test.UnitTests;

public class TestCreateBooksFromManningData(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void TestCreateManningBooks()
    {
        //SETUP

        //ATTEMPT
        var manningBooks = CreateBooksFromManningData.CreateSqlManningBooks(10).ToArray();

        //VERIFY
        _output.WriteLine($"Num Manning books created = {manningBooks.Count() }" + Environment.NewLine);

        foreach (var book in manningBooks)
        {
            _output.WriteLine(book + Environment.NewLine);
        }
    }

    [Fact]
    public void TestCreateManningBooks_ToDatabase()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookSqlDbContext>();
        using var context = new BookSqlDbContext(options);
        context.Database.EnsureClean();

        var manningBooks = CreateBooksFromManningData.CreateSqlManningBooks(100).ToArray();

        //ATTEMPT
        context.AddRange(manningBooks);
        context.SaveChanges();

        //VERIFY
        context.ChangeTracker.Clear();
        _output.WriteLine($"Num books: {context.Books.Count()}");
        _output.WriteLine($"Num Authors: {context.Authors.Count()}");
        _output.WriteLine($"Num Tags: {context.Tags.Count()}");
    }
}