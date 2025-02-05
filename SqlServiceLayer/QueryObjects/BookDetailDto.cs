﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Html;

namespace SqlServiceLayer.QueryObjects
{
    public class BookDetailDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public DateOnly PublishedOn { get; set; }
        public bool EstimatedDate { get; set; }
        public decimal OrgPrice { get; set; }
        public decimal ActualPrice { get; set; }
        public string PromotionText { get; set; }
        public string AuthorsOrdered { get; set; }
        public string[] TagStrings { get; set; }
        public string ImageUrl { get; set; }
        public string ManningBookUrl { get; set; }

        public HtmlString Description { get; set; }
        public HtmlString AboutAuthor { get; set; }
        public HtmlString AboutReader { get; set; }
        public HtmlString AboutTechnology { get; set; }
        public HtmlString WhatsInside { get; set; }
    }
}