// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace SqlDataLayer.Classes
{
    public class Author
    {
        public int AuthorId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; }

        //------------------------------
        //Relationships

        public ICollection<Book> Books { get; set; }
        public List<BookAuthor> BookAuthors { get; set; }
    }
}