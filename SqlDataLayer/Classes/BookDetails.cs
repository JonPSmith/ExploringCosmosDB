// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using System.ComponentModel.DataAnnotations.Schema;

namespace SqlDataLayer.Classes;

[Table("Books")]
[NotMapped]
public class BookDetails
{
    public const string NoDetailsAvailable = "No extra details for this book";

    public string Description { get; set; }
    public string AboutAuthor { get; set; }
    public string AboutReader { get; set; }
    public string AboutTechnology { get; set; }
    public string WhatsInside { get; set; }
}