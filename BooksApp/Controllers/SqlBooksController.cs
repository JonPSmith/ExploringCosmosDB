using BooksApp.Models;
using Microsoft.AspNetCore.Mvc;
using SqlServiceLayer;
using System.Diagnostics;
using CommonServiceLayer;
using SqlDataLayer.SqlBookEfCore;

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
            if (_context.Books.Any())
                return View(new BookListCombinedDto(new SortFilterPageOptions(), null));
            
            return View();
        }
    }
}
