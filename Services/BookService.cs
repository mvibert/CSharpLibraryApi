using LibraryApi.Data;
using LibraryApi.Dtos;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services;

// [Scoped] in Program.cs : one instance by HTTP request, like the "request" scope
// implicit in a Spring @Service that depends on an EntityManager/Session.
public class BookService(LibraryDbContext db) : IBookService
{
    public async Task<IReadOnlyList<BookResponse>> SearchAsync(BookSearchQuery query, CancellationToken ct)
    {
        // IQueryable : nothing is executed in database before the final ToListAsync.
        // It's the equivalent of a Specification/Criteria API built dynamically.
        IQueryable<Book> books = db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Title))
            books = books.Where(b => EF.Functions.Like(b.Title, $"%{query.Title}%"));

        if (!string.IsNullOrWhiteSpace(query.Author))
            books = books.Where(b => EF.Functions.Like(b.Author, $"%{query.Author}%"));

        if (!string.IsNullOrWhiteSpace(query.Genre))
            books = books.Where(b => b.Genre != null && EF.Functions.Like(b.Genre, $"%{query.Genre}%"));

        if (!string.IsNullOrWhiteSpace(query.Isbn))
            books = books.Where(b => b.Isbn == query.Isbn);

        if (query.Available.HasValue)
            books = books.Where(b => b.IsAvailable == query.Available.Value);

        var results = await books.OrderBy(b => b.Title).ToListAsync(ct);
        return results.Select(ToResponse).ToList();
    }

    public async Task<BookResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var book = await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
        return book is null ? null : ToResponse(book);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken ct)
    {
        if (await db.Books.AnyAsync(b => b.Isbn == request.Isbn, ct))
            throw new DuplicateIsbnException(request.Isbn);

        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Isbn = request.Isbn,
            Genre = request.Genre,
            PublicationYear = request.PublicationYear,
            IsAvailable = true,
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(ct); // flush + implicit commit, equivalent to repository.save()

        return ToResponse(book);
    }

    public async Task<BookResponse?> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (book is null)
            return null;

        var isbnTaken = await db.Books.AnyAsync(b => b.Isbn == request.Isbn && b.Id != id, ct);
        if (isbnTaken)
            throw new DuplicateIsbnException(request.Isbn);

        book.Title = request.Title;
        book.Author = request.Author;
        book.Isbn = request.Isbn;
        book.Genre = request.Genre;
        book.PublicationYear = request.PublicationYear;
        book.IsAvailable = request.IsAvailable;

        await db.SaveChangesAsync(ct); // book is "tracked" : EF detects the modified fields (dirty checking)
        return ToResponse(book);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (book is null)
            return false;

        db.Books.Remove(book);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static BookResponse ToResponse(Book b) =>
        new(b.Id, b.Title, b.Author, b.Isbn, b.Genre, b.PublicationYear, b.IsAvailable);
}
