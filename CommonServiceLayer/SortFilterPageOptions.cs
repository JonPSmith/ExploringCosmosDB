// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace CommonServiceLayer;

public class SortFilterPageOptions
{

    // Sorting
    public SortByOptions SortByOptions { get; set; }
    public SortByOptions SortBy { get; set; } = SortByOptions.SimpleOrder;

    //Filtering
    public FilterByOptions FilterByOptions { get; set; }
    public FilterByOptions FilterBy { get; set; } = FilterByOptions.NoFilter;
    public string FilterName { get; set; }
    public Dictionary<Enum, List<DropdownTuple>> FilterItemsDictionary { get; set; }

    //Paging
    public int PageIndex { get; set; }
    public int[] PageSizes = [5, 10, 20, 50, 100, 500, 1000];
    public int PageSize { get; set; } = 100;

    //Information on the paging
    public int PageNum { get; set; } = 1;
    public int NumPages { get; set; }







    /// <summary>
    ///     This holds the state of the key parts of the SortFilterPage parts
    /// </summary>
    public string PrevCheckState { get; set; }


    public async Task SetupRestOfDtoAsync<T>(IQueryable<T> query)
    {
        SetupRestOfDto(await query.CountAsync());
    }

    public void SetupRestOfDto(int rowCount)
    {
        NumPages = (int)Math.Ceiling(
            ((double)rowCount) / PageSize);
        PageNum = Math.Min(
            Math.Max(1, PageNum), NumPages);

        var newCheckState = GenerateCheckState();
        if (PrevCheckState != newCheckState)
            PageNum = 1;

        PrevCheckState = newCheckState;
    }

    //----------------------------------------
    //private methods

    /// <summary>
    ///     This returns a string containing the state of the SortFilterPage data
    ///     that, if they change, should cause the PageNum to be set back to 0
    /// </summary>
    /// <returns></returns>
    private string GenerateCheckState()
    {
        return $"{(int)FilterBy},{FilterName},{PageSize},{NumPages}";
    }
}