// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CosmosDataLayer.CosmosBookEfCore;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using Test.Helpers;
using TestSupport.EfHelpers;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests;

public class TestCosmosCreateBooksFromManningData(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void TestCreateSqlManningBooks_OneBook()
    {
        //SETUP
        var cosmosBooks = CreateCosmosBooksFromManningData.CosmosManningBooks_OneBigClass(1);

        //ATTEMPT
        var book = cosmosBooks.First();

        //VERIFY
        _output.WriteLine(book.ToString());
        book.ToString().ShouldEqual("Graph Databases in Action by Dave Bechberger, Josh Perryman. " +
                                    "Price 49.99, no reviews, Published by Manning publications on 15/11/2020, Tags: Data");
    }

    [Fact]
    public async Task TestCreateCosmosOneBigClass_ToDatabase()
    {
        //SETUP
        var options = "OneBigOne".CreateCosmosOptions();
        using var context = new BookCosmosContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        var cosmosBooks = CreateCosmosBooksFromManningData.CosmosManningBooks_OneBigClass(14);

        //ATTEMPT
        await context.AddRangeAsync(cosmosBooks);
        await context.SaveChangesAsync();

        //VERIFY
        context.ChangeTracker.Clear();
        var books = await context.OneBigBooks.ToListAsync();
        foreach (var book in books)
        {
            _output.WriteLine(book.ToString());
        }

    }
}