// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer.Dtos;
using CommonServiceLayer;

namespace CosmosServiceLayer;

public class IListBooksCosmosService
{
    public interface IListBooksSqlService
    {
        Task<IQueryable<BookListDto>> SortFilterPageAsync(SortFilterPageOptions options);
    }
}