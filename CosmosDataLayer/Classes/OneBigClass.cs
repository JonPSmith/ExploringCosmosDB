// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using SqlDataLayer.Classes;

namespace CosmosDataLayer.Classes;

public class OneBigClass
{
    public const int PromotionalTextLength = 200;

    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Title { get; set; }

    public DateOnly PublishedOn { get; set; }

    public string Publisher { get; set; }

    public decimal OrgPrice { get; set; }
    public decimal ActualPrice { get; set; }

    [MaxLength(PromotionalTextLength)]
    public string PromotionText { get; set; }

    [MaxLength(200)]
    public string ImageUrl { get; set; }

    /// <summary>
    /// This contains the url to get to the Manning version of the book
    /// </summary>
    public string ManningBookUrl { get; set; }

    //---------------------------------------
    //relationships

    public List<CosmosReviews> CosmosReviews { get; set; }
    public List<string> Authors { get; set; }
    public List<string> Tags { get; set; }
    public PriceOffer Promotion { get; set; }
    public CosmosDetails Details { get; set; }

    public override string ToString()
    {
        var authors = String.Join(", ", Authors);
        var authorString = string.Join(", ", authors);
        var reviewsString = CosmosReviews.Any()
            ? $"{CosmosReviews.Count()} reviews, stars = {CosmosReviews.Average(item => item.NumStars):#.##}"
            : "no reviews";

        var tagsString = !Tags.Any()
            ? "No tags"
            : "Tags: " + String.Join(", ", Tags);

        return $"{Title} by {authorString}. Price {ActualPrice}, {reviewsString}," +
               $" Published by {Publisher} on {PublishedOn:d}, {tagsString}";
    }
}