// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CosmosDataLayer.Classes;

namespace GenerateBooks;

public static class CreateCosmosBooksFromManningData
{
    public static IEnumerable<OneBigClass> CosmosManningBooks_OneBigClass(int numBooks, int maxReviewsPerBook = 12)
    {
        var creator = new CreateSqlBooksFromManningData();
        var manningBooks = creator.CreateSqlManningBooks(numBooks, maxReviewsPerBook);

        foreach (var book in manningBooks)
        {

            var cosmosBook = new OneBigClass
            {
                Title = book.Title,
                PublishedOn = book.PublishedOn,
                Publisher = book.Publisher,
                OrgPrice = book.OrgPrice,
                ActualPrice = book.OrgPrice,
                ImageUrl = book.ImageUrl,
                ManningBookUrl = book.ManningBookUrl,
                //lists of strings
                Authors = book.Authors.Select(x => x.Name).ToList(),
                Tags = book.Tags.Select(x => x.TagId).ToList(),
                //now the relationships 
                CosmosReview = CosmosReview.ReviewsToCosmosReviews(book.Reviews).ToList(),
                Promotion = book.Promotion,         //one-to-one, or null
                Details = book.Details              //one-to-one, or null
            };

            yield return cosmosBook;
        }
    }
}