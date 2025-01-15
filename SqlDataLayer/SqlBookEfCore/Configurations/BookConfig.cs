// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlDataLayer.Classes;

namespace SqlDataLayer.SqlBookEfCore.Configurations
{
    internal class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> entity)
        {
            entity.HasIndex(x => x.PublishedOn);
            entity.HasIndex(x => x.ActualPrice);

            // //Had to manually configure a BookTag because EfCore.GenericServices can't (yet) handle index entities
            // entity.HasMany(x => x.Tags)
            //     .WithMany(x => x.Books)
            //     .UsingEntity<BookTag>(
            //         x => x.HasOne(x => x.Tag).WithMany().HasForeignKey(y => y.TagId),
            //         x => x.HasOne(x => x.Book).WithMany().HasForeignKey(y => y.BookId));

        }
    }
}