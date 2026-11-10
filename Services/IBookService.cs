using LibraryApi.Dtos;

namespace LibraryApi.Services;

// The interface = the contract, injected into the controller (like a @Service interface).
// We'll come back to this: it's what allows us to easily mock in unit tests.
public interface IBookService
{
    Task<IReadOnlyList<BookResponse>> SearchAsync(BookSearchQuery query, CancellationToken ct);
    Task<BookResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken ct);
    Task<BookResponse?> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}

// Business exception thrown when the ISBN already exists (unique constraint in the database).
// The controller translates it into a 409 Conflict.
public class DuplicateIsbnException(string isbn) : Exception($"A book with the ISBN '{isbn}' already exists.");
