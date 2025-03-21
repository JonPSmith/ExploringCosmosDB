// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CosmosDataLayer.CosmosBookEfCore;
using GenerateBooks;
using Microsoft.EntityFrameworkCore;

namespace Test.Helpers;

public static class CosmosDbHelpers
{
    public static DbContextOptions<BookCosmosContext> CreateCosmosOptions(this string databaseName,
        bool useEmulator = true)
    {
        if (!useEmulator)
            throw new Exception("Only using the cosmos emulator for now");

        var optionsBuilder = new DbContextOptionsBuilder<BookCosmosContext>();
        optionsBuilder.UseCosmos(
            "https://localhost:8081",
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            databaseName);
        return optionsBuilder.Options;
    }

    public static async Task WipeAndFillCosmosDatabaseAsync(this DbContextOptions<BookCosmosContext> options, int numBooks)
    {
        using var context = new BookCosmosContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        var cosmosBooks = CreateCosmosBooksFromManningData.CosmosManningBooks_OneBigClass(numBooks);

        await context.AddRangeAsync(cosmosBooks);
        await context.SaveChangesAsync();
    }

    public static async Task SetupCosmosContext(this string databaseName, int numBooks, bool useEmulator = true)
    {
        var options = CreateCosmosOptions(databaseName, useEmulator);
        using var context = new BookCosmosContext(options);

        //only create the data if the database is empty
        if (await context.Database.CanConnectAsync())
        {
            if (context.OneBigBooks.Count() != 0)
                return;
        }

        //Need to fill the database with sample data
        await WipeAndFillCosmosDatabaseAsync(options, numBooks);
    }
}