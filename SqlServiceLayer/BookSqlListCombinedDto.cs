// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using SqlServiceLayer.Dtos;

namespace SqlServiceLayer
{
    public class BookSqlListCombinedDto
    {
        public BookSqlListCombinedDto(SortFilterPageOptions sortFilterPageData, IEnumerable<BookSqlListDto> booksList)
        {
            SortFilterPageData = sortFilterPageData;
            BooksList = booksList;
        }

        public SortFilterPageOptions SortFilterPageData { get; private set; }

        public IEnumerable<BookSqlListDto> BooksList { get; private set; }
    }
}