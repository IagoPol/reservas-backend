using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Modelos;

namespace ReservasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CancelacionesController : ControllerBase // CancelacionesController.cs handles CRUD operations for the Cancelacion model. Incluimos some logic to ensure that cancellations refer to valid reservations. Recordemos que Cancelacion.cs contiene public Reserva? Reserva { get; set; } y tmb contiene la fk ReservaId.
    {
        // propiedad de tipo mi AppDbContext :
        private readonly AppDbContext _contexto;

        // constructor parametrizado con dicha propiedad:
        public CancelacionesController(AppDbContext contexto)
        { 
            _contexto = contexto;
        }

        // POST (Create), GET (Read), PUT (Update), DELETE (Delete) para mi tabla de cancelaciones.

        // POST: api/cancelaciones
        [HttpPost]
        public async Task<ActionResult<Cancelacion>> CrearCancelacion(Cancelacion nuevacancelacion)
        {
            // Validación opcional: ensure the referenced Reserva exists (recordemos que en modelo de cancelaciones hay una fk que es ReservaId)
            var existeLaReserva = await _contexto.Reservas.AnyAsync(r => r.ReservaId == nuevacancelacion.ReservaId);
            if (!existeLaReserva)
            {
                return BadRequest($"NO existe una reserva con el ID {nuevacancelacion.ReservaId}");
            }

            _contexto.Cancelaciones.Add(nuevacancelacion);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCancelacion), new { id = nuevacancelacion.CancelacionId }, nuevacancelacion);
        } // end of CrearCancelacion method

        // GET: api/cancelaciones
        // GetCancelaciones tiene por propósito retornar una lista de todas las cancelaciones from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cancelacion>>> GetCancelaciones()
        { 
            var cancelaciones = await _contexto.Cancelaciones.Include(c => c.Reserva).ToListAsync();

            return Ok(cancelaciones);
        } // end of GetCancelaciones method

        // GET: api/cancelaciones/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Cancelacion>> GetCancelacion(int idcancelacion)
        {
            var cancelacion = await _contexto.Cancelaciones.Include(c => c.Reserva).FirstOrDefaultAsync(c => c.CancelacionId == idcancelacion);
            // var pago = await _contexto.Pagos.Include(p => p.Reserva).FirstOrDefaultAsync(p => p.PagoId == idpago); puse en PagosControllers.cs
            // var instalacion = await _contexto.Instalaciones.FindAsync(idinstalacion); puse en InstalacionesControllers.

            if (cancelacion == null)
            {
                return NotFound();
            }

            return Ok(cancelacion);
        } // end of GetCancelacion method

        // PUT: api/cancelaciones/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCancelacion(int idurl, Cancelacion cancelacion)
        {
            if (idurl != cancelacion.CancelacionId)
            { 
                return BadRequest("El ID de la URL NO coincide con el ID de la cancelación");
            }

            _contexto.Entry(cancelacion).State = EntityState.Modified;

            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_contexto.Cancelaciones.Any(e => e.CancelacionId == idurl))
                {
                    return NotFound();
                }
                throw;
            } // end of catch

            return NoContent();
        } // end of ActualizarCancelacion method

        // DELETE: api/cancelaciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCancelacion(int idcancelacion)
        {
            var cancelacionToDelete = await _contexto.Cancelaciones.FindAsync(idcancelacion);

            if (cancelacionToDelete == null)
            {
                return NotFound();
            }

            _contexto.Cancelaciones.Remove(cancelacionToDelete);
            await _contexto.SaveChangesAsync();

            return NoContent();
        } // end of method EliminarCancelacion

    } // end of clase CancelacionesController
}
