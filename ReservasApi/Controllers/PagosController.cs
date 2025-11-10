using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Modelos;

namespace ReservasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase // PagosController es the controller that will manage CRUD operations for my Pago model, which is linked to my Reserva model via the ReservaId foreign key.
    { // we'll include the standard endpoints (GET, GET by ID, POST, PUT, DELETE) and also handle the navigation property (public Reserva? Reserva { get; set; }) to make sure EFCore behaves correctly (esto último no lo mencionó para los controller de instalaciones, reservas y usuarios).
        // propiedad de tipo mi AppDbContext :
        private readonly AppDbContext _contexto;

        // constructor parametrizado con dicha propiedad:
        public PagosController(AppDbContext contexto)
        { 
            _contexto = contexto;
        }

        // POST (Create), GET (Read), PUT (Update), DELETE (Delete) para mi tabla de pagos.

        // POST: api/pagos
        [HttpPost]
        public async Task<ActionResult<Pago>> CrearPago(Pago pago)
        { 
            // Nos aseguramos de que la reserva existe antes de crear su pago correspondiente :
            var existeLaReserva = await _contexto.Reservas.AnyAsync(r => r.ReservaId == pago.ReservaId);

            if (!existeLaReserva) // si la reserva no existe
            {
                return BadRequest($"NO existe una reserva con el ID {pago.ReservaId}");
            }

            // Habiendo comprobado que existe la reserva con el id indicando, entonces creamos su pago:
            _contexto.Pagos.Add(pago);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPago), new { id = pago.PagoId }, pago);
        } // end of method CrearPago

        // GET: api/pagos
        // GetPagos tiene por propósito retornar una lista de todos los pagos from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
        {
            // Incluimos los datos de la reserva relacionada (al pago) (esta inclusión es opcional, pero útil para debugguear)
            var pagos = await _contexto.Pagos.Include(p => p.Reserva).ToListAsync(); // p.Reserva se puede hacer gracias a que puse public Reserva? Reserva { get; set; } en Pago.cs

            return Ok(pagos);
        } // end of GetPagos method.

        // GET: api/pagos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> GetPago(int idpago)
        {
            var pago = await _contexto.Pagos.Include(p => p.Reserva).FirstOrDefaultAsync(p => p.PagoId == idpago);
            // var instalacion = await _contexto.Instalaciones.FindAsync(idinstalacion); puse en InstalacionesControllers.

            if (pago == null)
            {
                return NotFound();
            }

            return Ok(pago);
        } // end of GetPago method

        // PUT: api/pagos/{id} (actualizar datos de un pago de una reserva)
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPago(int idurl, Pago pago)
        {
            if (idurl != pago.PagoId)
            { 
                return BadRequest("El ID de la URL NO coincide con el ID del pago");
            }

            _contexto.Entry(pago).State = EntityState.Modified;

            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_contexto.Pagos.Any(e => e.PagoId == idurl))
                {
                    return NotFound();
                }
                throw;
            } // end of catch

            return NoContent();
        } // end of ActualizarPago method.

        // DELETE: api/pagos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPago(int pagoid)
        {
            var pagoToDelete = await _contexto.Pagos.FindAsync(pagoid);

            if (pagoToDelete == null)
            {
                return NotFound();
            }

            _contexto.Pagos.Remove(pagoToDelete);
            await _contexto.SaveChangesAsync();

            return NoContent();
        } // end of EliminarPago method

    }
}
