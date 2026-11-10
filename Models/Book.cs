namespace LibraryApi.Models;

// Equivalent of a JPA entity (@Entity). No annotation needed here :
// EF Core recognizes "Id" as the primary key by convention (like @Id + @GeneratedValue(IDENTITY)).
public class Book
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Author { get; set; }

    // ISBN-13 : we keep it as a string (can contain dashes).
    public required string Isbn { get; set; }

    public string? Genre { get; set; }

    public int? PublicationYear { get; set; }

    public bool IsAvailable { get; set; } = true;
}
