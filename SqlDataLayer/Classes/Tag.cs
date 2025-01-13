// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace SqlDataLayer.Classes
{
    public class Tag
    {
        [MaxLength(40)]
        public string TagId { get; set; }

        public IReadOnlyCollection<Book> Books { get; set; }
    }
}