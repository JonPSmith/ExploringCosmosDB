﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using CommonServiceLayer;
using CommonServiceLayer.Dtos;

namespace SqlServiceLayer
{
    public interface IListBooksSqlService
    {
        Task<IQueryable<BookListDto>> SortFilterPageAsync(SortFilterPageOptions options);
    }
}