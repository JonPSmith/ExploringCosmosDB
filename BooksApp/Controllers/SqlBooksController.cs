using Microsoft.AspNetCore.Mvc;
using SqlServiceLayer;
using CommonServiceLayer;
using SqlDataLayer.SqlBookEfCore;
using Microsoft.EntityFrameworkCore;
using SqlServiceLayer.Dtos;
using SqlServiceLayer.QueryObjects;

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

    public async Task<IActionResult> Index(SortFilterPageOptions options, [FromServices] IListBooksSqlService sqlService)
    {
        var bookList = await (await sqlService.SortFilterPageAsync(options))
            .ToListAsync();

        return View(new BookSqlListCombinedDto(options, bookList));
    }

    public async Task<IActionResult> Detail(int id)
    {
        return View(await BookSqlDetail.GetBookDetailAsync(_context, id));
    }

    /// <summary>
    /// This provides the filter search dropdown content
    /// </summary>
    /// <param name="options"></param>
    /// <param name="service"></param>
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