// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CosmosDataLayer.Classes;
using Microsoft.EntityFrameworkCore;

namespace CosmosDataLayer.CosmosBookEfCore;

public class BookCosmosContext : DbContext
{
    public BookCosmosContext(DbContextOptions<BookCosmosContext> options)
        : base(options) { }

    public DbSet<OneBigClass> OneBigBooks { get; set; }
}