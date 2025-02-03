using Microsoft.AspNetCore.Mvc;
using SqlServiceLayer;
using CommonServiceLayer;
using SqlDataLayer.SqlBookEfCore;
using Microsoft.EntityFrameworkCore;
using SqlServiceLayer.Dtos;

namespace BooksApp.Controllers;

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

    /// <summary>
    /// This provides the filter search dropdown content
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    [HttpGet]
    public JsonResult GetFilterSearchContent(SortFilterPageOptions options, [FromServices] IBookSqlFilterDropdownService service)
    {
        var traceIdent = HttpContext.TraceIdentifier;
        return Json(
            new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent,
                service.GetSqlFilterDropDownValues(
                    options.FilterBy)));
    }
}