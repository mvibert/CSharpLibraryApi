using LibraryApi.Dtos;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

// [ApiController] = combo of @RestController + automatic validation of the ModelState.
// [Route] = @RequestMapping("/api/books") on the class.
[ApiController]
[Route("api/books")]
public class BooksController(IBookService bookService) : ControllerBase
{
    // GET /api/books                -> complete list (no pagination for now)
    // GET /api/books?title=hobbit   -> search by title (partial, case-insensitive)
    // GET /api/books?author=tolkien&available=true -> combination of criteria
    // The binding of BookSearchQuery from the query string is automatic (like @RequestParam on each field, but grouped in an object).
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookResponse>>> Search(
        [FromQuery] BookSearchQuery query, CancellationToken ct)
    {
        var books = await bookService.SearchAsync(query, ct);
        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponse>> GetById(int id, CancellationToken ct)
    {
        var book = await bookService.GetByIdAsync(id, ct);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create(
        [FromBody] CreateBookRequest request, CancellationToken ct)
    {
        // If [ApiController] detects an invalid ModelState (ex: Title vide),
        // it short-circuits before even entering here and returns 400 automatically.
        try
        {
            var created = await bookService.CreateAsync(request, ct);
            // 201 Created + header Location: /api/books/{id}, like ResponseEntity.created(uri).
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DuplicateIsbnException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookResponse>> Update(
        int id, [FromBody] UpdateBookRequest request, CancellationToken ct)
    {
        try
        {
            var updated = await bookService.UpdateAsync(id, request, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (DuplicateIsbnException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await bookService.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

}
