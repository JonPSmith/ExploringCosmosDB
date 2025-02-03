// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using SqlServiceLayer.Dtos;
using System;

namespace SqlServiceLayer.QueryObjects
{

    public static class BookListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<BookListDto> FilterBooksBy(
            this IQueryable<BookListDto> books,
            BooksFilterBy filterBy, string filterValue) 
        {
            if (string.IsNullOrEmpty(filterValue)) 
                return books; 

            switch (filterBy)
            {
                case BooksFilterBy.NoFilter: 
                    return books; 
                case BooksFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue); 
                    return books.Where(x => 
                        x.ReviewsAverageVotes > filterVote);
                case BooksFilterBy.ByTags:
                    return books.Where(x => x.TagStrings.Any(y => y == filterValue));
                case BooksFilterBy.ByPublicationYear:
                    if (filterValue == AllBooksNotPublishedString) 
                        return books.Where( 
                            x => x.PublishedOn > DateOnly.FromDateTime(DateTime.UtcNow) ); 

                    var filterYear = int.Parse(filterValue); 
                    return books.Where( 
                        x => x.PublishedOn.Year == filterYear 
                             && x.PublishedOn <= DateOnly.FromDateTime(DateTime.UtcNow)); 
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }
    }
}