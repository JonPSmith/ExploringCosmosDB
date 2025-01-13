// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using StatusGeneric;

namespace SqlDataLayer.Classes
{
    public class Book
    {
        public const int PromotionalTextLength = 200;
        
        public int BookId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        public DateOnly PublishedOn { get; set; }
        public bool EstimatedDate { get; set; }

        public string Publisher { get; set; }
        public decimal OrgPrice { get; set; }
        public decimal ActualPrice { get; set; }

        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; set; }

        [MaxLength(200)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// This contains the url to get to the Manning version of the book
        /// </summary>
        public string ManningBookUrl { get; set; }


        //---------------------------------------
        //relationships

        public IReadOnlyCollection<Review> Reviews  { get; set; }
        public IReadOnlyCollection<BookAuthor> AuthorsLink { get; set; }
        public IReadOnlyCollection<Tag> Tags { get; set; }
        public BookDetails Details { get; set; }
        public string AuthorsOrdered { get; set; }

        //----------------------------------------------

        public override string ToString()
        {
            var authors = AuthorsLink?.OrderBy(x => x.Order).Select(x => x.Author.Name);
            var authorString = string.Join(", ", authors);
            var reviewsString = Reviews.Any()
                ? $"{Reviews.Count()} reviews, stars = {Reviews.Average(item => item.NumStars):#.##}"
                : "no reviews";

            var tagsString = Tags == null || !Tags.Any()
                ? ""
                : $" Tags: " + string.Join(", ", Tags.Select(x => x.TagId));

            return $"{Title}: by {authorString}. Price {ActualPrice}, {reviewsString}. Published {PublishedOn:d}{tagsString}";
        }
    }

}