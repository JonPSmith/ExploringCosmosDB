// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace SqlDataLayer.Classes
{
    public class Review
    {
        public const int NameLength = 100;

        public int ReviewId { get; set; }

        [MaxLength(NameLength)]
        public string VoterName { get; set; }

        public byte NumStars { get; set; }
        public string Comment { get; set; }

        //-----------------------------------------
        //Relationships

        public Book Book { get; set; }
    }

}