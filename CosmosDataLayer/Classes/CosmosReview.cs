// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using SqlDataLayer.Classes;
using System.ComponentModel.DataAnnotations;

namespace CosmosDataLayer.Classes;

public class CosmosReview
{
    public const int NameLength = 100;

    [MaxLength(NameLength)]
    public string VoterName { get; set; }

    public byte NumStars { get; set; }
    public string Comment { get; set; }

    public static IEnumerable<CosmosReview> ReviewsToCosmosReviews(IEnumerable<Review> reviews)
    {
        foreach (var review in reviews)
        {
            yield return new CosmosReview
            {
                Comment = review.Comment,
                NumStars = review.NumStars,
                VoterName = review.VoterName
            };
        }
    }
}