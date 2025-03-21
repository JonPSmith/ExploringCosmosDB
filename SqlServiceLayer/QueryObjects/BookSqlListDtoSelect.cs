// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using SqlDataLayer.Classes;
using CommonServiceLayer.Dtos;

namespace SqlServiceLayer.QueryObjects
{
    public static class BookSqlListDtoSelect
    {
        public static IQueryable<BookListDto> 
            MapBookToDto(this IQueryable<Book> books) 
        {
            return books.Select(p      => new BookListDto 
            {
                BookId                 = p.BookId,           
                Title                  = p.Title,            
                PublishedOn            = p.PublishedOn,      
                OrgPrice               = p.OrgPrice,         
                ActualPrice            = p.ActualPrice,      
                PromotionText          = p.PromotionalText,  
                AuthorsOrdered         = string.Join(", ", 
                    p.BookAuthors                          
                        .OrderBy(q     => q.Order)         
                        .Select(q      => q.Author.Name)), 
                TagStrings             = p.Tags            
                    .Select(x => x.TagId).ToArray(),       
                ReviewsCount           = p.Reviews.Count(), 
                ReviewsAverageVotes    =                    
                    p.Reviews.Select(y =>                   
                        (double?)y.NumStars).Average(),   
                ReviewsAverageVotesCached = p.ReviewsAverageVotesCache,
                ManningBookUrl         = p.ManningBookUrl
            });
        }

    }
}