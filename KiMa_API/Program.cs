
using Microsoft.EntityFrameworkCore;
using KiMa_API.Data;

var builder = WebApplication.CreateBuilder(args);

// ?? 1. Verbindung zur MySQL-Datenbank herstellen
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// ?? 2. API-Controller aktivieren
builder.Services.AddControllers();

// ?? 3. OpenAPI/Swagger aktivieren (falls gewünscht)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ?? 4. OpenAPI/Swagger nur in Development anzeigen
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ?? 5. Middleware hinzufügen
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ?? 6. App starten
app.Run();

