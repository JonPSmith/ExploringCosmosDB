// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Xunit.Abstractions;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using TestSupport.Attributes;
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

    //!!! did this to get data on the BookApp 
    [RunnableInDebugOnly]
    public void FillSqkManningBooksToBookApp()
    {
        //SETUP
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ExploringCosmosDB-Sql;" +
                               "Trusted_Connection=True;MultipleActiveResultSets=true";
        var builder = new DbContextOptionsBuilder<BookSqlDbContext>();
        builder.UseSqlServer(connectionString);
        var context = new BookSqlDbContext(builder.Options);

        var manningBooks = CreateBooksFromManningData.CreateSqlManningBooks();

        //ATTEMPT
        context.AddRange(manningBooks);
        context.SaveChanges();

        //VERIFY
    }
}