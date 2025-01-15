// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SqlDataLayer.Classes;

public class PriceOffer
{
    public const int PromotionalTextLength = 200;

    public int PriceOfferId { get; set; }

    [Column(TypeName = "decimal(9,2)")]
    public decimal NewPrice { get; set; }

    [MaxLength(PromotionalTextLength)]
    public string PromotionalText { get; set; }

    //-----------------------------------------------
    //Relationships

    public int BookId { get; set; }
}