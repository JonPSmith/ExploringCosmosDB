﻿// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Newtonsoft.Json;
using SqlDataLayer.Classes;
using Microsoft.IdentityModel.Tokens;
using TestSupport.Helpers;
using SqlDataLayer;
using System.Text.RegularExpressions;

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

            var book = CreateSqlBooks.CreateBook(jsonBook.title, publishedOn, PublisherString,
                price, fullImageUrl, authors, tags);

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

    private static Dictionary<string, Author> NormalizeAuthorsToCommaDelimited(List<ManningBooksJson> summaryJson)
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