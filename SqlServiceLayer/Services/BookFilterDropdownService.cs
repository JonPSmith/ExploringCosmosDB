// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using SqlDataLayer.SqlBookEfCore;
using SqlServiceLayer.Dtos;
using SqlServiceLayer.QueryObjects;

namespace SqlServiceLayer.Services;

public class BookSqlFilterDropdownService : IBookSqlFilterDropdownService
{
    private readonly BookSqlDbContext _db;

    public BookSqlFilterDropdownService(BookSqlDbContext db)
    {
        _db = db;
    }

    /// <summary>
    ///     This makes the various Value + text to go in the dropdown based on the FilterBy option
    /// </summary>
    /// <param name="filterBy"></param>
    /// <returns></returns>
    public IEnumerable<DropdownTuple> GetSqlFilterDropDownValues(BooksFilterBy filterBy)
    {
        switch (filterBy)
        {
            case BooksFilterBy.NoFilter:
                //return an empty list
                return new List<DropdownTuple>();
            case BooksFilterBy.ByVotes:
                return FormVotesDropDown();
            case BooksFilterBy.ByTags:
                return _db.Tags
                    .Select(x => new DropdownTuple
                    {
                        Value = x.TagId,
                        Text = x.TagId
                    }).ToList();
            case BooksFilterBy.ByPublicationYear:
                var comingSoon = _db.Books.                      
                    Any(x => x.PublishedOn > DateOnly.FromDateTime(DateTime.Today));
                var result = _db.Books 
                    .Where(x => x.PublishedOn <= DateOnly.FromDateTime(DateTime.Today)) 
                    .Select(x => x.PublishedOn.Year)             
                    .Distinct()                                  
                    .OrderByDescending(x => x)
                    .Select(x => new DropdownTuple               
                    {                                            
                        Value = x.ToString(),                    
                        Text = x.ToString()                      
                    }).ToList();                                 
                if (comingSoon)
                    result.Insert(0, new DropdownTuple
                    {
                        Value = BookSqlListDtoFilter.AllBooksNotPublishedString,
                        Text = BookSqlListDtoFilter.AllBooksNotPublishedString
                    });

                return result;
            default:
                throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
        }
    }

    //------------------------------------------------------------
    // private methods

    private static IEnumerable<DropdownTuple> FormVotesDropDown()
    {
        return new[]
        {
            new DropdownTuple {Value = "4", Text = "4 stars and up"},
            new DropdownTuple {Value = "3", Text = "3 stars and up"},
            new DropdownTuple {Value = "2", Text = "2 stars and up"},
            new DropdownTuple {Value = "1", Text = "1 star and up"},
        };
    }
}