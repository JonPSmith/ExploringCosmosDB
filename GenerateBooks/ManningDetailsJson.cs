// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace GenerateBooks
{
    public class ManningDetailsJson
    {
                                                        //BookDetails class matching to this Json data
        public string aboutAuthor { get; set; }         //AboutAuthor
        public string aboutReader { get; set; }         //AboutReader
        public string aboutTechnology { get; set; }     //BboutTechnology
        public string description { get; set; }         //Description
        public string externalId { get; set; }
        public int productId { get; set; }              //used to link to ManningDetail's productId
        public string title { get; set; }
        public string whatsInside { get; set; }         //WhatsInside
    }
}