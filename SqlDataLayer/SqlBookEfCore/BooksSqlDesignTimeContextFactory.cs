// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SqlDataLayer.SqlBookEfCore;

public class BooksSqlDesignTimeContextFactory : IDesignTimeDbContextFactory<BookSqlDbContext>
{
    //This connection string MUST match the SqlBooksConnection connection string in the BooksApp's appsettings.json
    private const string ConnectionString =
        "Server=(localdb)\\mssqllocaldb;Database=ExploringCosmosDB-Sql;Trusted_Connection=True;MultipleActiveResultSets=true";

    /// <summary>Creates a new instance of a derived context.</summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>An instance of <typeparamref name="TContext" />.</returns>
    public BookSqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BookSqlDbContext>();
        optionsBuilder.UseSqlServer(ConnectionString, 
            b => b.MigrationsAssembly("BooksApp"));

        return new BookSqlDbContext(optionsBuilder.Options);
    }
}
/******************************************************************************
 * NOTES ON MIGRATION:
 *
 * BookApp.UI has two application DbContexts, BookDbContext and OrderDbContest
 * Each has its own project, migrations and migration history table
 * You need to build a migration from the DbContext's project (see below)
 *
 * NOTE: The EF Core commands give an error, but it does create the migration
 *
 * see https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/migrations?tabs=visual-studio
 *
 * The following NuGet libraries must be loaded
 * 1. Add to BookApp: "Microsoft.EntityFrameworkCore.Tools"
 * 2. Add to SqlDataLayer: "Microsoft.EntityFrameworkCore.SqlServer"
 *
 * 2. Using Package Manager Console commands
 * The steps are:
 *
 * a) Make sure the default project is BookApp
 * b) Use the PMC command
 *    Add-Migration NameForMigration -Context BookSqlDbContext -OutputDir Migrations
 * c) Use PMC command
 *    Update-database (or migrate on startup if you add the IDesignTimeDbContextFactory service )
 *
 * If you want to start afresh then:
 * a) Delete the current database
 * b) Delete all the class in the Migration directory
 * c) follow the steps to add a migration
 ******************************************************************************/