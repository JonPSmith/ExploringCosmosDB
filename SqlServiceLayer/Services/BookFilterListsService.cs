// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using SqlDataLayer.SqlBookEfCore;

namespace SqlServiceLayer.Services;

public class BookFilterListsService
{
    public Dictionary<Enum, List<DropdownTuple>> FilterItemsDictionary { get; private set; } =
        new Dictionary<Enum, List<DropdownTuple>>();

    public BookFilterListsService(BookSqlDbContext context)
    {
        FilterItemsDictionary.Add(FilterByOptions.NoFilter,
            new List<DropdownTuple> { new DropdownTuple { Value = "0", Text = "" } });
        FilterItemsDictionary.Add(FilterByOptions.ByVotes, new List<DropdownTuple>
        {
            new DropdownTuple { Value = "4", Text = "4 stars and up" },
            new DropdownTuple { Value = "3", Text = "3 stars and up" },
            new DropdownTuple { Value = "2", Text = "2 stars and up" },
            new DropdownTuple { Value = "1", Text = "1 star and up" }
        });

        var tagsDropdowns = context.Tags
            .Select(x => new DropdownTuple
            {
                Value = x.TagId,
                Text = x.TagId
            }).ToList();
        FilterItemsDictionary.Add(FilterByOptions.ByTags, new List<DropdownTuple>(context.Tags
            .Select(x => new DropdownTuple
            {
                Value = x.TagId,
                Text = x.TagId
            }).ToList()));

        FilterItemsDictionary.Add(FilterByOptions.ByPublicationYear, new List<DropdownTuple>(context.Books
            .Where(x => x.PublishedOn <= DateOnly.FromDateTime(DateTime.Today))
            .Select(x => x.PublishedOn.Year)
            .Distinct()
            .OrderByDescending(x => x)
            .Select(x => new DropdownTuple
            {
                Value = x.ToString(),
                Text = x.ToString()
            }).ToList()));
    }
}
