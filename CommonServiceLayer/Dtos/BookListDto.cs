﻿// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace CommonServiceLayer.Dtos;

public class BookListDto
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public DateOnly PublishedOn { get; set; }
    public decimal OrgPrice { get; set; }
    public decimal ActualPrice { get; set; }
    public string PromotionText { get; set; }
    public string AuthorsOrdered { get; set; }
    public string[] TagStrings { get; set; }
    public int ReviewsCount { get; set; }
    public double? ReviewsAverageVotes { get; set; }
    public double ReviewsAverageVotesCached { get; set; }
    public string ManningBookUrl { get; set; }
}