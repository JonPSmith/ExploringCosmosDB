﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using SqlDataLayer.Classes;
using SqlServiceLayer.Dtos;

namespace SqlServiceLayer.QueryObjects
{
    public static class BookSqlListDtoSelect
    {
        public static IQueryable<BookSqlListDto> 
            MapBookToDto(this IQueryable<Book> books) 
        {
            return books.Select(p      => new BookSqlListDto //#A
            {
                BookId                 = p.BookId,           //#A
                Title                  = p.Title,            //#A
                PublishedOn            = p.PublishedOn,      //#A
                OrgPrice               = p.OrgPrice,         //#B
                ActualPrice            = p.ActualPrice,      //#B
                PromotionText          = p.PromotionalText,  //#B
                AuthorsOrdered         = string.Join(", ", //#C
                    p.BookAuthors                          //#C
                        .OrderBy(q     => q.Order)         //#C
                        .Select(q      => q.Author.Name)), //#C
                TagStrings             = p.Tags            //#C
                    .Select(x => x.TagId).ToArray(),       //#C
                ReviewsCount           = p.Reviews.Count(), //#D
                ReviewsAverageVotes    =                    //#D
                    p.Reviews.Select(y =>                   //#D
                        (double?)y.NumStars).Average(),     //#D
                ManningBookUrl         = p.ManningBookUrl
            });
        }

        /*********************************************************
        #A Good practice: Only load the properties you need
        #B Good practice: Part 3 uses DDD so that the price promotion alters the ActualPrice 
        #C Good practice: Don't load the whole relationships, but just the parts you need 
        #D Good practice: The ReviewsCount and ReviewsAverageVotes are calculated in the database
        * *******************************************************/
    }
}