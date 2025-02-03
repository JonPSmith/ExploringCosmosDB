using Microsoft.AspNetCore.Mvc;
using SqlServiceLayer;
using CommonServiceLayer;
using SqlDataLayer.SqlBookEfCore;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index(SortFilterPageOptions options, [FromServices] IListBooksService service)
        {
            var bookList = await (await service.SortFilterPageAsync(options))
                .ToListAsync();

            return View(new BookListCombinedDto(options, bookList));
        }
    }
}
