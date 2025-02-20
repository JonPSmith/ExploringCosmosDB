using System.ComponentModel.DataAnnotations;

namespace CommonServiceLayer
{
    public enum SortByOptions
    {
        [Display(Name = "sort by...")] SimpleOrder = 0,
        [Display(Name = "Votes ↑")] ByVotes,
        [Display(Name = "By Votes cached...")] ByVotesCache,
        [Display(Name = "Publication Date ↑")] ByPublicationDate,
        [Display(Name = "Price ↓")] ByPriceLowestFirst,
        [Display(Name = "Price ↑")] ByPriceHighestFirst
    }

    public enum FilterByOptions
    {
        [Display(Name = "All")] NoFilter = 0,
        [Display(Name = "By Votes...")] ByVotes,
        [Display(Name = "By Votes cached...")] ByVotesCache,
        [Display(Name = "By Tags...")] ByTags,
        [Display(Name = "By Year published...")]
        ByPublicationYear
    }
}
