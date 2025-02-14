// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using SqlServiceLayer.Dtos;

namespace SqlServiceLayer
{
    public interface IBookSqlFilterDropdownService
    {
        /// <summary>
        ///     This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterByOptions"></param>
        /// <returns></returns>
        IEnumerable<DropdownTuple> GetSqlFilterDropDownValues(FilterByOptions filterByOptions);
    }
}