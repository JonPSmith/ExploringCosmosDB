using SqlDataLayer.Classes;
using StatusGeneric;

namespace GenerateBooks;

public static class CreateBooks
{ 
    public static IStatusGeneric<Book> CreateBook(
        string title, DateOnly publishedOn,
        bool estimatedDate,
        string publisher, decimal price, string imageUrl,
        ICollection<Author> authors,
        ICollection<Tag> tags,
        BookDetails bookDetails)
    {
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (authors == null) throw new ArgumentNullException(nameof(authors));
        if (string.IsNullOrEmpty(title)) throw new ArgumentException("Value cannot be null or empty.", nameof(title));

        var status = new StatusGenericHandler<Book>();
        var book = new Book
        {
            Title = title,
            PublishedOn = publishedOn,
            EstimatedDate = estimatedDate,
            Publisher = publisher,
            OrgPrice = price,
            ActualPrice = price,
            ImageUrl = imageUrl,
            //We need to initialise the AuthorsOrdered string when the entry is created
            //NOTE: We must NOT initialise the ReviewsCount and the ReviewsAverageVotes as they default to zero
            AuthorsOrdered = string.Join(", ", authors.Select(x => x.Name)),

            Details = bookDetails,
            Tags = new HashSet<Tag>(tags),
            Reviews = new HashSet<Review>()
            
            //Reviews to be added when building test data
            //We need to initialise the AuthorsOrdered string when the entry is created
            //NOTE: We must NOT initialise the ReviewsCount and the ReviewsAverageVotes as they default to zero
        };
        
        byte order = 0;
        book.AuthorsLink = new HashSet<BookAuthor>(
            authors.Select(a =>
                new BookAuthor{Book = book, Author = a, Order = order++}));
        if (!book.AuthorsLink.Any())
            status.AddError(
                "You must have at least one Author for a book.");

        return status.SetResult(book);
    }

}