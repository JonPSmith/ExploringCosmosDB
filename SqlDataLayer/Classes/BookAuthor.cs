﻿// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace SqlDataLayer.Classes
{
    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public byte Order { get; set; }

        //-----------------------------
        //Relationships

        public Book Book { get; set; }
        public Author Author { get; set; }
    }
}