using System.Security.Cryptography.X509Certificates;
using SqlDataLayer.Classes;

namespace SqlDataLayer;

public static class CreateSqlBooks
{
    /// <summary>
    /// This creates a Book using the parameter data (the Author class has a null Email)
    /// NOTE: The following relationships have to be after this method
    /// - Reviews
    /// - Promotion
    /// - BookDetails
    /// </summary>
    /// <param name="title"></param>
    /// <param name="publishedOn"></param>
    /// <param name="publisher"></param>
    /// <param name="price"></param>
    /// <param name="imageUrl"></param>
    /// <param name="authorsNames"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Book CreateBook(
        string title, DateOnly publishedOn,
        string publisher, decimal price, string imageUrl,
        List<string> authorsNames,
        List<Tag> tags)
    {
        if (string.IsNullOrEmpty(title)) throw new ArgumentException("Value cannot be null or empty.", nameof(title));
        if (authorsNames == null || !authorsNames.Any()) throw new ArgumentException("Value cannot be null or empty.", nameof(title));
        
        var book = new Book
        {
            Title = title,
            Authors = new List<Author>(authorsNames.Select(x => new Author{Name = x})),
            PublishedOn = publishedOn,
            Publisher = publisher,
            OrgPrice = price,
            ActualPrice = price,
            ImageUrl = imageUrl,
            Tags = new HashSet<Tag>(tags)
        };

        book.BookAuthors = new List<BookAuthor>();
        byte order = 0;
        foreach (var author in book.Authors)
        {
            book.BookAuthors.Add(new BookAuthor { Book = book, Author = author, Order = order });
            order++;
        }

        return book;
    }

}