// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer.Dtos;
using CosmosDataLayer.Classes;

namespace CosmosServiceLayer.QueryObjects
{
    public static class OneBigListDtoSelect
    {
        public static IQueryable<BookListDto> 
            MapBookToDto(this IQueryable<OneBigClass> books) 
        {
            return books.Select(p      => new BookListDto
            {
                BookId             = p.Id,          
                Title              = p.Title,            
                PublishedOn        = p.PublishedOn,      
                OrgPrice           = p.OrgPrice,         
                ActualPrice        = p.ActualPrice,      
                PromotionText      = p.PromotionText,
                AuthorsOrdered     = string.Join(", ", p.Authors),
                TagStrings         = p.Tags.ToArray(),           
                ReviewsCount       = p.CosmosReview.Count,
                ReviewsAverageVotes    = p.CosmosReview.Any() 
                    ? p.CosmosReview.Average(item => item.NumStars) : 0,
                ManningBookUrl         = p.ManningBookUrl
            });
        }
    }
}