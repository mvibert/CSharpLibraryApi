using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Dtos;

// Input DTO for creation. The [Attributes] are the equivalent of annotations of type
// javax.validation (@NotBlank, @Size...). [ApiController] on the controller trigger the automatic validation and
// returns a 400 ProblemDetails, like @Valid + MethodArgumentNotValidException would do in Spring.
public class CreateBookRequest
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 1)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(97(8|9)-)?\d{9}(\d|X)$", ErrorMessage = "ISBN invalide (10 ou 13 chiffres dont en préfixe 978 ou 979 avec un tiret).")]
    public string Isbn { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Genre { get; set; }

    [Range(1450, 2100)]
    public int? PublicationYear { get; set; }
}

// Input DTO for update (PUT): we also allow changing the availability.
public class UpdateBookRequest : CreateBookRequest
{
    public bool IsAvailable { get; set; } = true;
}

// Output DTO. We avoid returning the EF entity directly to not expose the persistence implementation
// (same logic as in Spring: we map Entity -> DTO).
public record BookResponse(
    int Id,
    string Title,
    string Author,
    string Isbn,
    string? Genre,
    int? PublicationYear,
    bool IsAvailable);

// Search parameters, bound from the query string (?title=...&author=...).
public class BookSearchQuery
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    public string? Isbn { get; set; }
    public bool? Available { get; set; }
}
