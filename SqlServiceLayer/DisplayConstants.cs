﻿using System.ComponentModel.DataAnnotations;

namespace SqlServiceLayer
{
    public enum OrderByOptions
    {
        [Display(Name = "sort by...")] SimpleOrder = 0,
        [Display(Name = "Votes ↑")] ByVotes,
        [Display(Name = "Publication Date ↑")] ByPublicationDate,
        [Display(Name = "Price ↓")] ByPriceLowestFirst,
        [Display(Name = "Price ↑")] ByPriceHighestFirst
    }

    public enum BooksFilterBy
    {
        [Display(Name = "All")] NoFilter = 0,
        [Display(Name = "By Votes...")] ByVotes,
        [Display(Name = "By Tags...")] ByTags,
        [Display(Name = "By Year published...")]
        ByPublicationYear
    }

    public static class DisplayConstants
    {
        public const string AllBooksNotPublishedString = "Coming Soon";
    }
}
