// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Newtonsoft.Json;
using SqlDataLayer.Classes;
using Microsoft.IdentityModel.Tokens;
using TestSupport.Helpers;
using System.Text.RegularExpressions;

namespace GenerateBooks;

public static class CreateSqlBooksFromManningData
{
    private const string ImageUrlPrefix = "https://images.manning.com/360/480/resize/";
    public const string PublisherString = "Manning publications";

    /// <summary>
    /// This creates Books from the real-world data from Manning publications. To handle
    /// SQL books, where you can't have duplicable Author or Tag entity.
    /// Note: These generated books don't have any Reviews or PriceOffer entities. 
    /// </summary>
    /// <param name="numBooks">Optional: number books to take, if not set you get all books</param>
    /// <returns>Sql Books</returns>
    public static IEnumerable<Book> CreateSqlManningBooks(int numBooks = 1000)
    {
        //We take all the 
        var summaryFilePath = TestData.GetFilePath("ManningBooks-20200814.json");
        var summaryJson = JsonConvert.DeserializeObject<List<ManningBooksJson>>(
            File.ReadAllText(summaryFilePath)).Take(numBooks);
        var detailFilePath = TestData.GetFilePath("ManningDetails-20200723.json");
        var detailDict = JsonConvert.DeserializeObject<List<ManningDetailsJson>>(
                File.ReadAllText(detailFilePath))!.Take(numBooks)
            .ToDictionary(x => x.productId);

        var tagsNames = summaryJson.SelectMany(x => x.tags ?? []).Distinct().ToList();
        var tagsDict = tagsNames.ToDictionary(x => x, y => new Tag{TagId = y});
        var authorsDict = NormalizeAuthorsToCommaDelimited(summaryJson);

        foreach (var jsonBook in summaryJson.Where(x => !x.authorshipDisplay.IsNullOrEmpty()))
        {
            var fullImageUrl = ImageUrlPrefix + jsonBook.imageUrl;
            var publishedOn = DateOnly.FromDateTime(jsonBook.publishedDate ?? jsonBook.expectedPublishDate);
            var price = jsonBook.productOfferings.Any()
                ? jsonBook.productOfferings.Select(x => x.price).Max()
                : 100;
            var authors = jsonBook.authorshipDisplay.Split(',')
                .Select(x => authorsDict[x].Name).ToList();
            var tags = (jsonBook.tags ?? [])
                .Select(x => tagsDict[x]).ToList();

            //ATTEMPT
            var book = new Book
            {
                Title = jsonBook.title,
                Authors = new List<Author>(authors.Select(x => new Author { Name = x })),
                PublishedOn = publishedOn,
                Publisher = PublisherString,
                OrgPrice = price,
                ActualPrice = price,
                ImageUrl = fullImageUrl,
                Tags = new HashSet<Tag>(tags),
                BookAuthors = new List<BookAuthor>()
            };
            byte order = 0;
            foreach (var author in book.Authors)
            {
                book.BookAuthors.Add(new BookAuthor { Book = book, Author = author, Order = order });
                order++;
            }

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
            // else
            // {
            //     book.Details = new BookDetails
            //     {
            //         Description = BookDetails.NoDetailsAvailable
            //     };
            // }

            yield return book;
        }
    }

    private static Dictionary<string, Author> NormalizeAuthorsToCommaDelimited(IEnumerable<ManningBooksJson> summaryJson)
    {
        var authorDict = new Dictionary<string, Author>();
        foreach (var manningBooksJson in summaryJson)
        {
            var authors = NormalizeAuthorNames(manningBooksJson).ToList();
            manningBooksJson.authorshipDisplay = authors.Any()
                ? string.Join(',', authors)
                : null;
            authors.ForEach(name =>
            {
                if (!authorDict.ContainsKey(name))
                    authorDict[name] = new Author{Name = name};
            });
        }

        return authorDict;
    }

    //This decodes The authorshipDisplay string which contains lots of different formats
    internal static IEnumerable<string> NormalizeAuthorNames(ManningBooksJson json)
    {
        const string withChaptersBy = "With chapters selected by";
        //The formats for authors are
        //- Author1
        //- Author1 and Author2
        //- Author1, Author2
        //- Author1, Author2 with Author3
        //- Author1<br><i>Foreword by ...
        //- Author1 Edited by
        //- With chapters selected by ...
        //- contributions by
        //- Author1, Ph.D.
        //- null 

        if (json.authorshipDisplay == null)
            return new string[0];

        var authorString = json.authorshipDisplay.StartsWith(withChaptersBy)
            ? json.authorshipDisplay.Substring(withChaptersBy.Length)
            : json.authorshipDisplay;

        var breakIndex = authorString.IndexOf("<"); //<br><i>Foreword by 
        if (breakIndex > 0)
            authorString = authorString.Substring(0, breakIndex);
        var editedIndex = authorString.IndexOf("Edited by");
        if (editedIndex > 0)
            authorString = authorString.Substring(0, editedIndex);

        authorString = authorString
            .Replace("  ", " ")
            .Replace("Ph.D.", "")
            .Replace("contributions by", ",")
            .Replace(" with ", ",")
            .Replace(" and ", ",");
        if (Regex.Match(authorString, @";|#|&").Success)//Some name come out wrong - don't know why
            return new string[0];

        var authors = authorString.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(y => y.Trim());

        return authors;
    }
}