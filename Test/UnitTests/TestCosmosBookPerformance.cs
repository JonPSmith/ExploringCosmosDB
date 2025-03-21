// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer.Dtos;
using CosmosDataLayer.CosmosBookEfCore;
using CosmosServiceLayer.QueryObjects;
using Microsoft.EntityFrameworkCore;
using Test.Helpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests;

public class TestCosmosBookPerformance(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;
    private readonly int _numBooks = 10000;
    private readonly bool _seeAllLogString = false;
    
    //Use this when you want to change the number of books you have
    [RunnableInDebugOnly]
    public async Task ChangeTheDatabase()
    {
        var logs = new List<string>();
        var options = this.CreateUniqueMethodCosmosDbEmulatorWithLogTo<BookCosmosContext>(log => logs.Add(log));
        using var context = new BookCosmosContext(options);
        await options.WipeAndFillCosmosDatabaseAsync(_numBooks);
    }

    //----------------------------------------------------------------------------

    [Fact]
    public async Task TestSqlBook_ReadOnly()
    {
        //SETUP
        var logs = new List<string>();
        var options = this.CreateUniqueMethodCosmosDbEmulatorWithLogTo<BookCosmosContext>(log => logs.Add(log));
        using var context = new BookCosmosContext(options);

        //ATTEMPT
        BookListDto[] books;
        books = await context.OneBigBooks.MapBookToDto().ToArrayAsync();

        //VERIFY
        books.Length.ShouldEqual(_numBooks);
        //_output.WriteLine($"Total query time is {logs.FindTotalQueryTime()} ms.");
        foreach (var log in logs)
        {
            _output.WriteLine(log);
        }
    }
}