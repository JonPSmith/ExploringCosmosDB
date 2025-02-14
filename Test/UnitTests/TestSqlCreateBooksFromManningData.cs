// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Xunit.Abstractions;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using TestSupport.Attributes;
using TestSupport.EfHelpers;

namespace Test.UnitTests;

public class TestSqlCreateBooksFromManningData(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void TestCreateSqlManningBooks()
    {
        //SETUP

        //ATTEMPT
        var manningBooks = CreateSqlBooksFromManningData.CreateSqlManningBooks(10).ToArray();

        //VERIFY
        _output.WriteLine($"Num Manning books created = {manningBooks.Count() }" + Environment.NewLine);

        foreach (var book in manningBooks)
        {
            _output.WriteLine(book + Environment.NewLine);
        }
    }

    [Fact]
    public void TestCreateSqlManningBooks_ToDatabase()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookSqlDbContext>();
        using var context = new BookSqlDbContext(options);
        context.Database.EnsureClean();

        var manningBooks = CreateSqlBooksFromManningData.CreateSqlManningBooks(100).ToArray();

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