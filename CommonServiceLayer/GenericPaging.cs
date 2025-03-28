﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace CommonServiceLayer
{
    public static class GenericPaging
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query,
            int pageNumZeroStart, int pageSize)
        {
            if (pageSize == 0)
                throw new ArgumentOutOfRangeException
                    (nameof(pageSize), "pageSize cannot be zero.");

            if (pageNumZeroStart != 0)
                query = query.Skip(pageNumZeroStart * pageSize);

            return query.Take(pageSize); 
        }

    }
}