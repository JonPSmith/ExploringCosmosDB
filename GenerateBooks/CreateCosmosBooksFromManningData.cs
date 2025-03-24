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
        int id = 0;
        foreach (var book in manningBooks)
        {
            id ++;
            var cosmosBook = new OneBigClass
            {
                Id = id,                        //sets the id of the cosmos
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
                //now the simple relationships 
                Promotion = book.Promotion,         //one-to-one, or null
                
            };
            if (book.Details != null)
            {
                cosmosBook.Details = new CosmosDetails
                {
                    Id = id,
                    Description = book.Details.Description,
                    AboutAuthor = book.Details.AboutAuthor,
                    AboutReader = book.Details.AboutReader,
                    AboutTechnology = book.Details.AboutTechnology,
                    WhatsInside = book.Details.WhatsInside
                };
            }
            cosmosBook.CosmosReviews = new List<CosmosReviews>();
            foreach (var review in book.Reviews)
            {
                var cosmosReview = new CosmosReviews
                {
                    Id = id,
                    Comment = review.Comment,
                    NumStars = review.NumStars,
                    VoterName = review.VoterName
                };
                cosmosBook.CosmosReviews.Add(cosmosReview); 
            }

            yield return cosmosBook;
        }
    }
}