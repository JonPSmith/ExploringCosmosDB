// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Newtonsoft.Json;
using SqlDataLayer.Classes;
using Microsoft.IdentityModel.Tokens;
using TestSupport.Helpers;
using System.Text.RegularExpressions;


namespace GenerateBooks;

public class CreateSqlBooksFromManningData
{
    private const string ImageUrlPrefix = "https://images.manning.com/360/480/resize/";
    private const string PublisherString = "Manning publications";
    private const string ReplacementAuthor = "unknown author";

    private static readonly List<string> ReviewComments =
        //0               1                 2               3         4               5            6
        ["Terrible book", "Didn't like it", "It was so-so", "useful", "looking good", "very good", "WOW, great book"];

    //Random is used to create random review star ratings.
    //The pseudo-random is the same every time so that we can compare different tests  
    private static readonly Random Random = new Random(1);

    private List<ManningBooksJson> SummaryJson { get; init; }
    private Dictionary<int,ManningDetailsJson> DetailDict { get; init; }
    private Dictionary<string, Tag> TagsDict { get; init; }
    private Dictionary<string, Author> AuthorsDict { get; init; }

    //Useful data used when testing / performance code
    public int NumManningBooksJson { get; init; }
    public List<string> AllTagsNames { get; init; }

    /// <summary>
    /// This sets up the data to create many books
    /// </summary>
    public CreateSqlBooksFromManningData()
    {
        //This gets the Manning publications data about their books   
        var summaryFilePath = TestData.GetFilePath("ManningBooks-20200814.json");
        SummaryJson = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(summaryFilePath));
        NumManningBooksJson = SummaryJson.Count;
        var detailFilePath = TestData.GetFilePath("ManningDetails-20200723.json");
        DetailDict = JsonConvert.DeserializeObject<List<ManningDetailsJson>>(
                File.ReadAllText(detailFilePath))
            .ToDictionary(x => x.productId);

        //These hold a dictionary holding the Tags and Authors which are used when building SQL Books
        AllTagsNames = SummaryJson.SelectMany(x => x.tags ?? []).Distinct().ToList();
        TagsDict = AllTagsNames
            .ToDictionary(x => x, y => new Tag { TagId = y });
        AuthorsDict = NormalizeAuthorsToCommaDelimited(SummaryJson);
        AuthorsDict.Add(ReplacementAuthor, new Author{Name = ReplacementAuthor } );
    }

    /// <summary>
    /// This creates Books from the real-world data from Manning publications. To handle
    /// SQL books, where you can't have duplicable Author or Tag entity.
    /// Note: It also adds reviews and promotions
    /// </summary>
    /// <param name="numBooks">number books to take, if not set you get all books</param>
    /// <param name="maxReviewsPerBook">This sets the maximum reviews that a book can have</param>

    /// <returns>Sql Books</returns>
    public IEnumerable<Book> CreateSqlManningBooks(int numBooks, int maxReviewsPerBook = 12)
    {
        var makeBookTitlesDistinct = numBooks > SummaryJson.Count;
        for (int bookCount = 0; bookCount < numBooks; bookCount++)
        {
            var sectionNum = (int)Math.Truncate(bookCount / (double)SummaryJson.Count);
            var jsonBookIndex = bookCount % NumManningBooksJson;
            var jsonBook = SummaryJson[jsonBookIndex];

            //Remove books with no authors
            if (jsonBook.authorshipDisplay.IsNullOrEmpty())
                jsonBook.authorshipDisplay = "unknown author";
 
            var fullImageUrl = ImageUrlPrefix + jsonBook.imageUrl;
            var publishedOn = DateOnly.FromDateTime(jsonBook.publishedDate ?? jsonBook.expectedPublishDate);
            //the code below makes sure all the Books have a valid date
            if (publishedOn.Year < 1000)
                publishedOn = new DateOnly(2018, 1, 1);
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
            //Create reviews, including the ReviewsCount and ReviewsAverageVotes caches
            book.Reviews = new List<Review>(); 
            for (int j = 0; j < bookCount % maxReviewsPerBook; j++)
            {
                var numStars = (byte)Random.Next(1, 6);
                book.Reviews.Add(new Review{NumStars = numStars, 
                    Comment = ReviewComments[numStars], VoterName = $"User{bookCount:7}"});
            }
            book.ReviewsCount = book.Reviews?.Count ?? 0;
            if (book.Reviews?.Count > 0)
            {
                book.ReviewsCount = book.Reviews.Count;
                book.ReviewsAverageVotesCache = book.Reviews != null && book.Reviews.Any()
                    ? book.Reviews.Average(y => y.NumStars)
                    : 0.0;
            }
            book.ReviewsAverageVotesCache = book.Reviews != null && book.Reviews.Any()
                ? book.Reviews.Average(y => y.NumStars)
                : 0.0;

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