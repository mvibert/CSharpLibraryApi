using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data;

// Equivalent of a pair EntityManager + Hibernate Session. None  custom Repository is written : DbSet<Book> is used
// directly with LINQ (like a JpaRepository<Book, Integer> but without the automatic derived methods).
public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API : the equivalent of @Table / @Column / constraints in XML/annotations JPA.
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(b => b.Isbn).IsUnique();
            entity.Property(b => b.Title).HasMaxLength(200);
            entity.Property(b => b.Author).HasMaxLength(150);
            entity.Property(b => b.Isbn).HasMaxLength(20);
            entity.Property(b => b.Genre).HasMaxLength(60);
        });
    }
}
