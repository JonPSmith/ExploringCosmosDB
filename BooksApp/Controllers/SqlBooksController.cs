using BooksApp.Models;
using Microsoft.AspNetCore.Mvc;
using SqlServiceLayer;
using System.Diagnostics;
using CommonServiceLayer;

namespace BooksApp.Controllers
{
    public class SqlBooksController : Controller
    {
        private readonly ILogger<SqlBooksController> _logger;

        public SqlBooksController(ILogger<SqlBooksController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new BookListCombinedDto(new SortFilterPageOptions(), null));
        }
    }
}
