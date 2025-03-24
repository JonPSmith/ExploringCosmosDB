// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CosmosDataLayer.Classes;
using Microsoft.EntityFrameworkCore;

namespace CosmosDataLayer.CosmosBookEfCore;

public class BookCosmosContext : DbContext
{
    public const string PartitionKey = nameof(PartitionKey);

    public BookCosmosContext(DbContextOptions<BookCosmosContext> options)
        : base(options) { }

    public DbSet<OneBigClass> OneBigBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var oneBigModel = modelBuilder.Entity<OneBigClass>();
        oneBigModel.ToContainer(nameof(OneBigClass))
            .HasNoDiscriminator()
            .HasKey(d => d.Id);
        oneBigModel.Property<string>(PartitionKey);
        oneBigModel.HasPartitionKey(PartitionKey);

        var reviewsModel = modelBuilder.Entity<CosmosReviews>();
        reviewsModel.ToContainer(nameof(CosmosReviews))
            .HasNoDiscriminator()
            .HasKey(d => d.Id);
        reviewsModel.Property<string>(PartitionKey);
        reviewsModel.HasPartitionKey(PartitionKey);

        var detailsModel = modelBuilder.Entity<CosmosDetails>();
        detailsModel.ToContainer(nameof(CosmosDetails))
            .HasNoDiscriminator()
            .HasKey(d => d.Id);
        detailsModel.Property<string>(PartitionKey);
        detailsModel.HasPartitionKey(PartitionKey);
    }
}