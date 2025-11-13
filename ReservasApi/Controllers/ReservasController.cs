using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Modelos;
using ReservasApi.Modelos.DTO;

namespace ReservasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase // clase provided by ASP.NET Core MVC 
    {
        // Propiedad privada y de solo lectura:
        private readonly AppDbContext _contexto;

        // constructor parametrizado con la propiedad:
        public ReservasController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // Los HTTP methods POST, GET, PUT, DELETE correspond to Create, Read, Update and Delete, respectively. CRUD are the 4 basic operations you can perform on data in a database.

        // POST, GET, PUT, DELETE respecto de la tabla de reservas:

        // POST: api/reservas
        // CreateReserva(Reserva nuevareserva)
        // The following is the "create" (una nueva reserva) operation. Create reserva.
        [HttpPost] // matches POST /api/reservas
        public async Task<ActionResult<ReservaLeerDto>> CrearReserva(ReservaCrearDto dto)// (Reserva nuevareserva)
        { // the body of the request must contain a JSON representation of a Reserva (de la que quieres crear, supongo).
            // comienzo del código propio del DTO:
            // Validate that the user and installation exist :
            var usuario = await _contexto.Usuarios.FindAsync(dto.Dni); // Usuarios es un DbSet en mi AppDbContext.cs
            var instalacion = await _contexto.Instalaciones.FindAsync(dto.InstalacionId); // Instalaciones es un DbSet en mi dbcontext

            if (usuario == null || instalacion == null) // validamos that the user and installation exist.
            {
                return BadRequest("Usuario o Instalación NO encontrados.");
            }

            // Create the new booking and link navigation properties (para que 'usuario' e 'instalacion' -navigation properties del modelo de reservas- se actualicen en base a las foreign key 'dni' e 'instalacionId' en vez de quedarse null)
            var nuevareserva = new Reserva // each time you create a new reservation object in memory inside your controller, that instance starts with Usuario = null and Instalacion = null UNTIL YOU EXPLICITLY ASSIGN THEM.
            { 
                Dni = dto.Dni, InstalacionId = dto.InstalacionId, FechaHora = dto.FechaHora, NumeroAsistentes = dto.NumeroAsistentes,
                
                // Attach navigation properties (so that EF Core also attaches the Usuario and Instalacion entities based on fks 'dni' and 'instalacionId'):
                Usuario = usuario, // we're populating the existing nav props with the related entities ('usuario' and 'instalacion') fetched from the db.
                Instalacion = instalacion, // if you don't assign these nav props before saving, then when you later query the record, unless you use .Include(...), the nav props will remain 'null' becuase EFCore does not automatically load related data unless asked to.
            }; // explicitly assigning nav props affects writing/saving, whereas .Include(...) affects reading/querying. I should include both.
            
            // creates and saves a new Reserva en otras palabras Add and save:
            _contexto.Reservas.Add(nuevareserva); // EFCore adds it (la nueva reserva) to the database (Add + SaveChangesAsync()).
            await _contexto.SaveChangesAsync(); // after SaveChangesAsync(), the navigation properties are available and populated in memory. If you GET /api/reservas, you'll see the linked data (if you use .Include(...) in your GET).

            // Map to DTO for response:
            var reservaDto = new ReservaLeerDto
            {
                ReservaId = nuevareserva.ReservaId,
                FechaHora = nuevareserva.FechaHora,
                NumeroAsistentes = nuevareserva.NumeroAsistentes,
                Dni = nuevareserva.Dni,
                InstalacionId = nuevareserva.InstalacionId,

                Usuario = new UsuarioDto
                { 
                    Dni = usuario.Dni,
                    NombreCompleto = usuario.NombreCompleto
                },

                Instalacion = new InstalacionDto
                { 
                    InstalacionId = instalacion.InstalacionId,
                    Localizacion = instalacion.Localizacion
                }
            };

            // (Optional but clean) reload navigation properties to ensure they are fully populated and also ensures Swagger shows them in the response:
            // await _contexto.Entry(nuevareserva).Reference(r => r.Usuario).LoadAsync();
            // await _contexto.Entry(nuevareserva).Reference(r => r.Instalacion).LoadAsync();

            // Publish event to Kafka topic "reservas.creadas"
            // awai _kafkaProducer.PublishReservaCreadaAsync(nuevareserva);

            // Return Created (201)
            return CreatedAtAction(nameof(GetReservas), new { id = nuevareserva.ReservaId }, reservaDto); // antes del 13 de nov aquí ponía nameof(GetReserva)
        } // returns '201 Created' with the new object (la nueva reserva) and a Location header pointing to /api/reservas/{id}

        // GET (is the HTTP Method): api/reservas (esto se refiere a la URL)
        // GetReservas tiene por propósito retornar una lista de todas las reservas from the database.
        // This is the "read all" operation, GET: api/reservas. List all reservas.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservaLeerDto>>> GetReservas()
        {
            var reservas = await _contexto.Reservas // Reservas viene de public DbSet<Reserva> Reservas { get; set; } en AppDbContext.cs, refers to the Reservas table.
                .Include(r => r.Usuario) // "eager loading with .Include()
                .Include(r => r.Instalacion) // so EF Core brings in the related objects automatically ("related" to the fks 'dni' y 'instalacionId' I passed in Swagger POST).
                .ToListAsync(); // sin .Include este GET method would retrieve all bookings without including their related entities (Usuario and Instalacion), which would leave the nav props showing up as null in Swagger (y en la bd tmb, no?).
            // ToListAsync() asynchronously fetches all rows (de la tabla de reservas)

            // esto lo añado con intención de poder consultar get api reservas en swagger sin cycle problem:
            var resultado = reservas.Select(r => new ReservaLeerDto
            {
                ReservaId = r.ReservaId,
                FechaHora = r.FechaHora,
                NumeroAsistentes = r.NumeroAsistentes,
                Dni = r.Dni,
                InstalacionId = r.InstalacionId,

                Usuario = r.Usuario == null ? null : new UsuarioDto
                {
                    Dni = r.Usuario.Dni,
                    NombreCompleto = r.Usuario.NombreCompleto
                },

                Instalacion = r.Instalacion == null ? null : new InstalacionDto
                {
                    InstalacionId = r.Instalacion.InstalacionId,
                    Localizacion = r.Instalacion.Localizacion
                }
            }).ToList();

            return Ok(resultado); // el return "sends HTTP 200 with a JSON array of reservations".
        } // fin del GET /api/reservas

        // GET: api/reservas/{id} por ejemplo api/reservas/5
        // GetReserva(int id) a continuación:
        // The following is the "read one" (el que tenga por pk equis valor, pej el 5) operation. Get one reserva.
        [HttpGet("{id}")] // URL path includes an ID (like /api/reservas/5)
        public async Task<ActionResult<Reserva>> GetReserva(int idreserva)
        {
            var reserva = await _contexto.Reservas.FindAsync(idreserva); // fetches the record by primary key

            if (reserva == null) // if nothing (no reserva with that id) is found
            {
                return NotFound(); // return HTTP '404 Not Found'.
            }
            return Ok(reserva); // otherwise return '200 OK' with that reservation as JSON.
        } // end of GetReserva

        // PUT: api/reservas/5
        // UpdateReserva(int id, Reserva reserva)
        // This is the "update" operation. Update reserva.
        [HttpPut("{id}")] // matches PUT /api/reservas/{id}
        public async Task<IActionResult> ActualizarReserva(int idurl, Reserva reserva)
        {
            if (idurl != reserva.ReservaId) // ensures that the ID in the URL and (in) body match (for safety).
            {
                return BadRequest();
            }

            _contexto.Entry(reserva).State = EntityState.Modified; // marks the record as updated.

            try
            {
                await _contexto.SaveChangesAsync(); // supongo que esto es lo del "saves changes"
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_contexto.Reservas.Any(e => e.ReservaId == idurl))
                {
                    return NotFound();
                }
                throw;
            } // end of catch

            return NoContent(); // returns '204 No Content' to indicate success (no body needed)
        } // end of body of method ActualizarReserva

        // DELETE: api/reservas/{id} pej. api/reservas/5
        // This is the "delete" operation. Delete reserva.
        [HttpDelete("{id}")] // matches DELETE /api/reservas/{id}
        public async Task<IActionResult> EliminarReserva(int idreserva)
        {
            var reserva = await _contexto.Reservas.FindAsync(idreserva); // finds the record (la reserva) by ID

            if (reserva == null)
            {
                return NotFound();
            }

            _contexto.Reservas.Remove(reserva); // removes it (the record, la reserva) from the table (of reservas)
            await _contexto.SaveChangesAsync();

            return NoContent(); // returns '204 No Content'
        } // end of EliminarReserva method.

    } // end of public class ReservasController

    // Al respecto de todo el código anterior: "once this controller is registered in your app,
    // your backend will automatically expose a working REST API for the Reserva model"
}
