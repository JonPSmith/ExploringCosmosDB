// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using SqlDataLayer.Classes;
using SqlDataLayer.SqlBookEfCore;
using TestSupport.EfHelpers;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests;

public class TestBookSqlDbContext(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void BasicTestSqlBookContext()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookSqlDbContext>();
        using var context = new BookSqlDbContext(options);
        context.Database.EnsureClean();

        //ATTEMPT
        context.Add(new Book { Title = "New Book" });
        context.SaveChanges();

        //VERIFY
        context.ChangeTracker.Clear();
        context.Books.Count().ShouldEqual(1);
    }
}