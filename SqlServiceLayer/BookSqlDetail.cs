// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using SqlServiceLayer.QueryObjects;

namespace SqlServiceLayer;

public static class BookSqlDetail
{
    public static Task<BookSqlDetailDto> GetBookDetailAsync(BookSqlDbContext context, int bookId) =>
        context.Books.Select(p => new BookSqlDetailDto
        {
            BookId = p.BookId,
            Title = p.Title,
            PublishedOn = p.PublishedOn,
            OrgPrice = p.OrgPrice,
            ActualPrice = p.ActualPrice,
            PromotionText = p.PromotionalText,
            AuthorsOrdered = string.Join(", ",
                p.BookAuthors
                    .OrderBy(q => q.Order)
                    .Select(q => q.Author.Name)),
            TagStrings = p.Tags.Select(x => x.TagId).ToArray(),
            ImageUrl = p.ImageUrl,
            ManningBookUrl = p.ManningBookUrl,
            Description = new HtmlString(p.Details.Description),
            AboutAuthor = new HtmlString(p.Details.AboutAuthor),
            AboutReader = new HtmlString(p.Details.AboutReader),
            AboutTechnology = new HtmlString(p.Details.AboutTechnology),
            WhatsInside = new HtmlString(p.Details.WhatsInside)
        }).SingleOrDefaultAsync(x => x.BookId == bookId);
}