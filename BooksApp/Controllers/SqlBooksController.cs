using BooksApp.Models;
using Microsoft.AspNetCore.Mvc;
using SqlServiceLayer;
using System.Diagnostics;
using CommonServiceLayer;
using Microsoft.EntityFrameworkCore;
using SqlDataLayer;
using SqlDataLayer.SqlBookEfCore;
using SqlDataLayer.Classes;

namespace BooksApp.Controllers
{
    public class SqlBooksController : Controller
    {
        private readonly BookSqlDbContext _context;
        private readonly ILogger<SqlBooksController> _logger;

        public SqlBooksController(BookSqlDbContext context, ILogger<SqlBooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            //!!!
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Add(CreateSqlBooks.CreateBook(
                "Book title", new DateOnly(2025, 1, 13),
                "Manning", 123, null,
                new List<string> { "author1", "author2" },
                new List<Tag> { new Tag { TagId = "My Tag" } }));
            _context.SaveChanges();

            return View(new BookListCombinedDto(new SortFilterPageOptions(), null));
        }
    }
}
