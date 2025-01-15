// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


namespace SqlDataLayer.Classes
{
    public class BookDetails
    {
        public int BookDetailsId { get; set; }

        public string Description { get; set; }
        public string AboutAuthor { get; set; }
        public string AboutReader { get; set; }
        public string AboutTechnology { get; set; }
        public string WhatsInside { get; set; }

        public string AuthorsOrdered { get; set; }

        public int ReviewsCount { get; set; }

        public double ReviewsAverageVotes { get; set; }
    }
}   