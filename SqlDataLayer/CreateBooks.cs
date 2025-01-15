using SqlDataLayer.Classes;

namespace SqlDataLayer;

public static class CreateSqlBooks
{
   
    /// <summary>
    /// This creates a Book using the parameter data.
    /// NOTE: It doesn't set the following parts of the <see cref="Book"/> class
    /// - No BookDetails
    /// - The Author class has a null Email
    /// </summary>
    /// <param name="title"></param>
    /// <param name="publishedOn"></param>
    /// <param name="estimatedDate"></param>
    /// <param name="publisher"></param>
    /// <param name="price"></param>
    /// <param name="imageUrl"></param>
    /// <param name="authorsNames"></param>
    /// <param name="tags"></param>
    /// <param name="reviews"></param>
    /// <param name="promotion"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Book CreateBook(
        string title, DateOnly publishedOn,
        bool estimatedDate,
        string publisher, decimal price, string imageUrl,
        ICollection<string> authorsNames,
        ICollection<Tag> tags,
        ICollection<Review> reviews,
        PriceOffer promotion = null)
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
            Promotion = promotion
        };

        foreach (var review in reviews)
        {
            review.Book = book;
        }
        book.Reviews = reviews;

        byte order = 0;
        book.AuthorsLink = new HashSet<BookAuthor>(
            authorsNames.Select(a =>
                new BookAuthor{Book = book, Author = new Author{Name = a}, Order = order++}));
        if (!book.AuthorsLink.Any())
            throw new ArgumentException("Value cannot be null or empty.", nameof(authorsNames));

        return book;
    }

}