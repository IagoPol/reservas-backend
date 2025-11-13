using Microsoft.EntityFrameworkCore; // aparece al pone UseSqlServer
using ReservasApi.Data;

var builder = WebApplication.CreateBuilder(args); // esto ya venía por defecto.

// 1. Add services to the container.

// Add controllers (this enables the [ApiController] controllers)
builder.Services.AddControllers(); //.AddOpenApi();
    /*
    .AddJsonOptions(opciones => // esto lo añado con la intención de que get api reservas no me de cycle problem (el cual me impide ver la consulta)
    {
        opciones.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        opciones.JsonSerializerOptions.WriteIndented = true;
    });
    */

// Add EF Core and configure SQL Server using the connection string from appsettings.json (hijo de 'ReservasApi' folder -al igual que Program.cs-, hija de 'backend' folder)
builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")) // en appsettings.json puse "DefaultConnection": "Server=localhost,1433;Database=ReservasDb;User Id=sa;Password=docKer!8;TrustServerCertificate=True;"
);

// Add Swagger/OpenAPI support for testing your endpoints easily
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // requires installation of the Swagger (Swashbuckle) NuGet package.

var app = builder.Build(); // ya viene por defecto.

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // esto ya venía por defecto
{
    //app.MapOpenApi(); este es el único que venía por defecto.
    // Enable Swagger UI only in development :
    app.UseSwagger(); // esto lo puse yo
    app.UseSwaggerUI(); // esto lo puse yo
}

app.UseHttpsRedirection(); // ya venía por defecto.

app.UseAuthorization(); // lo puse yo.

// Map controller routes (this enables your /api/... endpoints)
app.MapControllers(); // lo puse yo.

app.Run(); // lo puse yo.

/* Todo esto venía por defecto.
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast", () =>
    {
        var forecast =  Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

    app.Run();

    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
*/
