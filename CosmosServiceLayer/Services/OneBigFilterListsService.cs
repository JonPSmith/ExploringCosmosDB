// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using CommonServiceLayer;
using CosmosDataLayer.CosmosBookEfCore;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosServiceLayer.Services;

public class OneBigFilterListsService
{
    public Dictionary<Enum, List<DropdownTuple>> FilterItemsDictionary { get; private set; } =
        new Dictionary<Enum, List<DropdownTuple>>();

    public OneBigFilterListsService(BookCosmosContext context)
    {
        FilterItemsDictionary.Add(FilterByOptions.NoFilter,
            new List<DropdownTuple> { new DropdownTuple { Value = "0", Text = "" } });
        FilterItemsDictionary.Add(FilterByOptions.ByVotes, new List<DropdownTuple>
        {
            new DropdownTuple { Value = "5", Text = "5 stars and up" },
            new DropdownTuple { Value = "4", Text = "4 stars and up" },
            new DropdownTuple { Value = "3", Text = "3 stars and up" },
            new DropdownTuple { Value = "2", Text = "2 stars and up" },
            new DropdownTuple { Value = "1", Text = "1 star and up" }
        });
        FilterItemsDictionary.Add(FilterByOptions.ByVotesCache, new List<DropdownTuple>
        {
            new DropdownTuple { Value = "5", Text = "5 stars and up" },
            new DropdownTuple { Value = "4", Text = "4 stars and up" },
            new DropdownTuple { Value = "3", Text = "3 stars and up" },
            new DropdownTuple { Value = "2", Text = "2 stars and up" },
            new DropdownTuple { Value = "1", Text = "1 star and up" }
        });

        //Use a dictionary to hold each distinct name  
        var tagNames = new Dictionary<string, int>();
        foreach (var tagsPerBook in context.OneBigBooks.Select(x => x.Tags))
        {
            foreach (var tag in tagsPerBook)
            {
                tagNames.Add(tag, 1);
            }
        }
        FilterItemsDictionary.Add(FilterByOptions.ByTags, tagNames.Keys
            .Select(x => new DropdownTuple
            {
                Value = x.ToString(),
                Text = x.ToString()
            }).ToList());

        FilterItemsDictionary.Add(FilterByOptions.ByPublicationYear, new List<DropdownTuple>(context.OneBigBooks
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
