// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using CommonServiceLayer.Dtos;
using CommonServiceLayer.Services;
using CosmosDataLayer.CosmosBookEfCore;
using CosmosServiceLayer.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace CosmosServiceLayer.Services
{
    public class ListBooksCosmosService : IListBooksCosmosService
    {
        private readonly BookCosmosContext _context;

        public ListBooksCosmosService(BookCosmosContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<BookListDto>> SortFilterPageAsync
            (SortFilterPageOptions options)
        {
            var booksQuery = _context.OneBigBooks
                .AsNoTracking() 
                .MapBookToDto() 
                .OrderBooksBy(options.SortByOptions) 
                .FilterBooksBy(options.FilterByOptions, options.FilterName); 

            return booksQuery.Page(options.PageNum - 1, options.PageSize); 
        }
    }


}