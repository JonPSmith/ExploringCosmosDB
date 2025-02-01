// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using SqlServiceLayer.Dtos;

namespace SqlServiceLayer;

public class BookListCombinedDto
{
    public BookListCombinedDto(SortFilterPageOptions sortFilterPageData, IEnumerable<BookListDto> booksList)
    {
        SortFilterPageData = sortFilterPageData;
        BooksList = booksList;
    }

    public SortFilterPageOptions SortFilterPageData { get; private set; }

    public IEnumerable<BookListDto> BooksList { get; private set; }
}