using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Modelos;

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
        public async Task<ActionResult<Reserva>> CrearReserva(Reserva nuevareserva)
        { // the body of the request must contain a JSON representation of a Reserva (de la que quieres crear, supongo).
            _contexto.Reservas.Add(nuevareserva); // EFCore adds it (la nueva reserva) to the database (Add + SaveChangesAsync()).
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = nuevareserva.ReservaId }, nuevareserva);
        } // returns '201 Created' with the new object (la nueva reserva) and a Location header pointing to /api/reservas/{id}

        // GET (is the HTTP Method): api/reservas (esto se refiere a la URL)
        // GetReservas tiene por propósito retornar una lista de todas las reservas from the database.
        // This is the "read all" operation, GET: api/reservas. List all reservas.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            var reservas = await _contexto.Reservas.ToListAsync(); // Reservas viene de public DbSet<Reserva> Reservas { get; set; } en AppDbContext.cs, refers to the Reservas table.
            // ToListAsync() asynchronously fetches all rows (de la tabla de reservas)
            return Ok(reservas); // el return "sends HTTP 200 with a JSON array of reservations".
        }

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
