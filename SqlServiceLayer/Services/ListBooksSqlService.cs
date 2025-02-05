// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using CommonServiceLayer;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer.SqlBookEfCore;
using SqlServiceLayer.Dtos;
using SqlServiceLayer.QueryObjects;

namespace SqlServiceLayer.Services
{
    public class ListBooksSqlService : IListBooksSqlService
    {
        private readonly BookSqlDbContext _context;

        public ListBooksSqlService(BookSqlDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<BookSqlListDto>> SortFilterPageAsync
            (SortFilterPageOptions options)
        {
            var booksQuery = _context.Books 
                .AsNoTracking() 
                .MapBookToDto() 
                .OrderBooksBy(options.OrderByOptions) 
                .FilterBooksBy(options.FilterBy, 
                    options.FilterValue); 

            await options.SetupRestOfDtoAsync(booksQuery); 

            return booksQuery.Page(options.PageNum - 1, options.PageSize); 
        }
    }


}