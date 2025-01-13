using SqlDataLayer.Classes;

namespace GenerateBooks;

public static class CreateBooks
{ 
    public static Book CreateSqlBook(
        string title, DateOnly publishedOn,
        bool estimatedDate,
        string publisher, decimal price, string imageUrl,
        ICollection<string> authorsNames,
        ICollection<Tag> tags)
    {
        if (string.IsNullOrEmpty(title)) throw new ArgumentException("Value cannot be null or empty.", nameof(title));
        if (authorsNames == null || !authorsNames.Any()) throw new ArgumentException("Value cannot be null or empty.", nameof(title));
        
        var book = new Book
        {
            Title = title,
            PublishedOn = publishedOn,
            EstimatedDate = estimatedDate,
            Publisher = publisher,
            OrgPrice = price,
            ActualPrice = price,
            ImageUrl = imageUrl,
            Tags = new HashSet<Tag>(tags),
            Reviews = new HashSet<Review>()
            
            //Reviews to be added when building test data
            //We need to initialise the AuthorsOrdered string when the entry is created
            //NOTE: We must NOT initialise the ReviewsCount and the ReviewsAverageVotes as they default to zero
        };
        
        byte order = 0;
        book.AuthorsLink = new HashSet<BookAuthor>(
            authorsNames.Select(a =>
                new BookAuthor{Book = book, Author = new Author{Name = a}, Order = order++}));
        if (!book.AuthorsLink.Any())
            throw new ArgumentException("Value cannot be null or empty.", nameof(authorsNames));

        return book;
    }

}