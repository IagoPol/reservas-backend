using Microsoft.EntityFrameworkCore; // aparece al pone UseSqlServer
using ReservasApi.Data;

// using ReservasApi.Eventos; // puse esta l�nea al poner con "Register the Kafka producer in DI (Dependency Injection)"

// para autenticación:
using Microsoft.AspNetCore.Authentication.JwtBearer; // no intellisense autocompletion
using Microsoft.IdentityModel.Tokens; // no intellisense autocompletion
using System.Text; // intellisense me acierta el Text.
using ReservasApi.Servicios;

var builder = WebApplication.CreateBuilder(args); // esto ya ven�a por defecto.

// 1. Add services to the container.

// Add controllers (this enables the [ApiController] controllers)
builder.Services.AddControllers(); //.AddOpenApi();
    /*
    .AddJsonOptions(opciones => // esto lo a�ado con la intenci�n de que get api reservas no me de cycle problem (el cual me impide ver la consulta)
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

// Register the Kafka producer in DI, para ello a�ado esta l�nea antes de var app = builder.Build() y despu�s de var builder = WebApplication.CreateBuilder(args):
//builder.Services.AddSingleton<IServicioProductorKafka, ProductorKafka>(); // ProductorKafka es una clase creada por m� que implementa la interfaz IServicioProductorKafka tmb creada por m� y que simplemente cuenta con un miembro, que es un m�todo.
// COMENTO LA L�NEA ANTERIOR DE CARA A EMPEZAR CON TESTS UNITARIOS IGNORAR TODO LO DE KAFKA.

// Insert the authentication setup above "app.Build()", along with your other builder.Services.*
// JWT Authentication:
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters // me autocompletó el primer TVP, no el segundo.
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]
                        ?? throw new Exception("Missing Jwt:Key in configuration") // because my appsettings.json contains "Jwt:Key", the left side of ?? is NOT null, therefore the exception is NOT thrown, the message is NEVER shown.
                )
            )
        };
    }); // lo que viene justo después de AddJwtBearer es una lambda that configures TokenValidationParameters.
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthService>();

// 2. Build the app
// in ASP.NET Core (including .NET 9), all service registrations must be added before:
var app = builder.Build(); // ya viene por defecto. Esta línea es toda la sección 2. Build the app

// 2. Configure the HTTP request pipeline. 
// A 25 de nov 2025 (al estar con lo de la autenticación), chatgpt a esto le llama 3. Configure middleware
if (app.Environment.IsDevelopment()) // esto ya ven�a por defecto
{
    //app.MapOpenApi(); este es el �nico que ven�a por defecto.
    // Enable Swagger UI only in development :
    app.UseSwagger(); // esto lo puse yo
    app.UseSwaggerUI(); // esto lo puse yo
}

app.UseHttpsRedirection(); // ya ven�a por defecto.

// Authentication must come before Authorization:
app.UseAuthentication();
app.UseAuthorization(); // lo puse yo.

// Map controller routes (this enables your /api/... endpoints)
app.MapControllers(); // lo puse yo.

app.Run(); // lo puse yo.

/* Todo esto ven�a por defecto.
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
