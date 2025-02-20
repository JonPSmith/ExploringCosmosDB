// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using SqlServiceLayer.Dtos;

namespace SqlServiceLayer.QueryObjects
{

    public static class BookSqlListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<BookSqlListDto> FilterBooksBy(
            this IQueryable<BookSqlListDto> books,
            FilterByOptions filterByOptions, string filterValue) 
        {
            if (string.IsNullOrEmpty(filterValue)) 
                return books; 

            switch (filterByOptions)
            {
                case FilterByOptions.NoFilter: 
                    return books; 
                case FilterByOptions.ByVotes:
                    var filterVote = int.Parse(filterValue); 
                    return books.Where(x => 
                        x.ReviewsAverageVotes > filterVote);
                case FilterByOptions.ByVotesCache:
                    var filterVoteCache = int.Parse(filterValue);
                    return books.Where(x =>
                        x.ReviewsAverageVotesCached > filterVoteCache);
                case FilterByOptions.ByTags:
                    return books.Where(x => x.TagStrings.Any(y => y == filterValue));
                case FilterByOptions.ByPublicationYear:
                    if (filterValue == AllBooksNotPublishedString) 
                        return books.Where( 
                            x => x.PublishedOn > DateOnly.FromDateTime(DateTime.UtcNow) ); 

                    var filterYear = int.Parse(filterValue); 
                    return books.Where( 
                        x => x.PublishedOn.Year == filterYear); 
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterByOptions), filterByOptions, null);
            }
        }
    }
}