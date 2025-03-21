// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer.Dtos;

namespace CommonServiceLayer.Services;

public static class BookListDtoSort
{
    public static IQueryable<BookListDto> OrderBooksBy
        (this IQueryable<BookListDto> books, SortByOptions sortByOptions)
    {
        switch (sortByOptions)
        {
            case SortByOptions.SimpleOrder: 
                return books.OrderByDescending( 
                    x => x.BookId); 
            case SortByOptions.ByVotes: 
                return books.OrderByDescending(x => 
                    x.ReviewsAverageVotes);
            case SortByOptions.ByVotesCache:
                return books.OrderByDescending(x =>
                    x.ReviewsAverageVotesCached);
            case SortByOptions.ByPublicationDate: 
                return books.OrderByDescending( 
                    x => x.PublishedOn); 
            case SortByOptions.ByPriceLowestFirst: 
                return books.OrderBy(x => x.ActualPrice); 
            case SortByOptions.ByPriceHighestFirst: 
                return books.OrderByDescending( 
                    x => x.ActualPrice); 
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(sortByOptions), sortByOptions, null);
        }
    }
}