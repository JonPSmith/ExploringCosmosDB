// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Newtonsoft.Json;
using SqlDataLayer.Classes;
using Microsoft.IdentityModel.Tokens;
using TestSupport.Helpers;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;

namespace GenerateBooks;

public class CreateSqlBooksFromManningData
{
    private const string ImageUrlPrefix = "https://images.manning.com/360/480/resize/";
    private const string PublisherString = "Manning publications";

    //Random is used to create random review star ratings.
    //The pseudo-random is the same every time so that we can compare different tests  
    private static readonly Random Random = new Random(1);

    private List<ManningBooksJson> SummaryJson { get; init; }
    private Dictionary<int,ManningDetailsJson> DetailDict { get; init; }
    private Dictionary<string, Tag> TagsDict { get; init; }
    private Dictionary<string, Author> AuthorsDict { get; init; }

    public CreateSqlBooksFromManningData(BookSqlDbContext context)
    {
        //This gets the Manning data which we use  
        var summaryFilePath = TestData.GetFilePath("ManningBooks-20200814.json");
        SummaryJson = JsonConvert.DeserializeObject<List<ManningBooksJson>>(
            File.ReadAllText(summaryFilePath));
        var detailFilePath = TestData.GetFilePath("ManningDetails-20200723.json");
        DetailDict = JsonConvert.DeserializeObject<List<ManningDetailsJson>>(
                File.ReadAllText(detailFilePath))
            .ToDictionary(x => x.productId);

        //These hold a dictionary holding the Tags and Authors which is 
        TagsDict = SummaryJson.SelectMany(x => x.tags ?? []).Distinct().ToList()
            .ToDictionary(x => x, y => new Tag { TagId = y });
        AuthorsDict = NormalizeAuthorsToCommaDelimited(SummaryJson);
    }


    /// <summary>
    /// This creates Books from the real-world data from Manning publications. To handle
    /// SQL books, where you can't have duplicable Author or Tag entity.
    /// Note: It also adds reviews and promotions
    /// </summary>
    /// <param name="numBooks">number books to take, if not set you get all books</param>
    /// <param name="maxReviewsPerBook">This sets the maximum reviews that a book can have</param>
    /// <param name="addPromotionEvery">This adds a Promotion on every addPromotionEvery book</param>
    /// <returns>Sql Books</returns>
    public IEnumerable<Book> CreateSqlManningBooks(int numBooks, 
        int maxReviewsPerBook = 12, int addPromotionEvery = 7)
    {
        var makeBookTitlesDistinct = numBooks > SummaryJson.Count;

        foreach (var jsonBook in SummaryJson.Where(x => !x.authorshipDisplay.IsNullOrEmpty()))
        {
            var fullImageUrl = ImageUrlPrefix + jsonBook.imageUrl;
            var publishedOn = DateOnly.FromDateTime(jsonBook.publishedDate ?? jsonBook.expectedPublishDate);
            var price = jsonBook.productOfferings.Any()
                ? jsonBook.productOfferings.Select(x => x.price).Max()
                : 100;
            var authors = jsonBook.authorshipDisplay.Split(',')
                .Select(x => AuthorsDict[x].Name).ToList();
            var tags = (jsonBook.tags ?? [])
                .Select(x => TagsDict[x]).ToList();

            var book = new Book
            {
                Title = makeBookTitlesDistinct ? $"{jsonBook.title} (copy {sectionNum})" : jsonBook.title,
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

            if (DetailDict.ContainsKey(jsonBook.id))
            {
                book.Details = new BookDetails
                {
                    Description = DetailDict[jsonBook.id].description,
                    AboutAuthor = DetailDict[jsonBook.id].aboutAuthor,
                    AboutReader = DetailDict[jsonBook.id].aboutReader,
                    AboutTechnology = DetailDict[jsonBook.id].aboutTechnology,
                    WhatsInside = DetailDict[jsonBook.id].whatsInside,
                };
            }

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