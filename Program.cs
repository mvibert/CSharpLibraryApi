using LibraryApi.Data;
using LibraryApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Saves services in the container DI ---
// It's the equivalent of Spring's component scanning (@Component/@Service/@Repository)
// but explicit: each service is declared here with its lifetime.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // generates JSON OpenAPI consumed by the Swagger UI

// AddDbContext = "Scoped" by default : one instance of DbContext by HTTP request,
// like a Hibernate Session linked to the request.
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Library")
        ?? "Data Source=library.db"));

// Scoped : one instance per request (the default in Spring for a @Service bean).
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

// Applies migrations of EF Core at startup (equivalent of Flyway/Liquibase
// launched automatically, or spring.jpa.hibernate.ddl-auto=update with more control).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // available on /swagger, like /swagger-ui.html with springdoc
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
