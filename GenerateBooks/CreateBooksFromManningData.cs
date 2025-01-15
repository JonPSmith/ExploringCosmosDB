// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Newtonsoft.Json;
using SqlDataLayer.Classes;
using Microsoft.IdentityModel.Tokens;
using TestSupport.Helpers;
using SqlDataLayer;

namespace GenerateBooks;

public static class CreateBooksFromManningData
{
    private const string ImageUrlPrefix = "https://images.manning.com/360/480/resize/";
    public const string PublisherString = "Manning publications";

    public static IEnumerable<Book> CreateManningBooks()
    {
        var summaryFilePath = TestData.GetFilePath("ManningBooks-20200814.json");
        var summaryJson = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(summaryFilePath));
        var detailFilePath = TestData.GetFilePath("ManningDetails-20200723.json");
        var detailDict = JsonConvert.DeserializeObject<List<ManningDetailsJson>>(
                File.ReadAllText(detailFilePath))
            .ToDictionary(x => x.productId);

        var tagsNames = summaryJson.SelectMany(x => x.tags ?? []).Distinct().ToList();
        var tagsDict = tagsNames.ToDictionary(x => x, y => new Tag{TagId = y});

        foreach (var jsonBook in summaryJson.Where(x => !x.authorshipDisplay.IsNullOrEmpty()))
        {
            var fullImageUrl = ImageUrlPrefix + jsonBook.imageUrl;
            var publishedOn = DateOnly.FromDateTime(jsonBook.publishedDate ?? jsonBook.expectedPublishDate);
            var price = jsonBook.productOfferings.Any()
                ? jsonBook.productOfferings.Select(x => x.price).Max()
                : 100;
            var authors = jsonBook.authorshipDisplay.Split(',');
            var tags = (jsonBook.tags ?? [])
                .Select(x => tagsDict[x]).ToList();

            var book = CreateSqlBooks.CreateBook(jsonBook.title, publishedOn, PublisherString,
                price, fullImageUrl, authors, tags, null);

            if (detailDict.ContainsKey(jsonBook.id))
            {
                book.Details = new BookDetails
                {
                    Description = detailDict[jsonBook.id].description,
                    AboutAuthor = detailDict[jsonBook.id].aboutAuthor,
                    AboutReader = detailDict[jsonBook.id].aboutReader,
                    AboutTechnology = detailDict[jsonBook.id].aboutTechnology,
                    WhatsInside = detailDict[jsonBook.id].whatsInside,
                };
            }
            else
            {
                book.Details = new BookDetails
                {
                    Description = BookDetails.NoDetailsAvailable
                };
            }

            yield return book;
        }
    }

}