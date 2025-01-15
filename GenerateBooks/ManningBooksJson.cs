// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace GenerateBooks;

public class ManningBooksJson
{
                                                                //BookDetails class matching to this Json data   
    public DateTime expectedPublishDate { get; set; }           //PublishedOn
    public bool hasAudio { get; set; }
    public string authorshipDisplay { get; set; }               //Author's names, comma delimited 
    public string isbn { get; set; }                            //ISBN
    public string externalId { get; set; }
    public DateTime lastSignificantDate { get; set; }
    public string title { get; set; }                           //Title   
    public string seoKeywords { get; set; }                     
    public string[] tags { get; set; }                          //Tags, Array of strings
    public bool livebook { get; set; }
    public Productoffering[] productOfferings { get; set; }
    public string subtitle { get; set; }
    public string imageUrl { get; set; }                        //Last part image Url
    public bool isLiveVideo { get; set; }
    public bool isLiveProject { get; set; }
    public int id { get; set; }                                 //used to link to ManningBook's productId
    public DateTime? publishedDate { get; set; }
    public string slug { get; set; }
}

public class Productoffering
{
    public float price { get; set; }
    public int id { get; set; }
    public string productType { get; set; }
}

