using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Modelos;

namespace ReservasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstalacionesController : ControllerBase
    {
        // propiedad de tipo mi AppDbContext:
        private readonly AppDbContext _contexto;

        // constructor parametrizado con dicha propiedad:
        public InstalacionesController(AppDbContext contexto)
        { 
            _contexto = contexto;
        }

        // POST (Create), GET (Read), PUT (Update), DELETE (idem) respecto de la tabla de instalaciones:

        // POST: api/instalaciones
        [HttpPost]
        public async Task<ActionResult<Instalacion>> CrearInstalacion(Instalacion nuevainstalacion)
        {
            _contexto.Instalaciones.Add(nuevainstalacion);
            await _contexto.SaveChangesAsync();

            // 201 Created response + Location header pointing to the new resource :
            return CreatedAtAction(nameof(GetInstalacion), new { id = nuevainstalacion.InstalacionId }, nuevainstalacion);
        } // end of CrearInstalacion method.

        // GET: api/instalaciones
        // GetInstalaciones tiene por propósito retornar una lista de todas las instalaciones from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instalacion>>> GetInstalaciones()
        {
            var instalaciones = await _contexto.Instalaciones.ToListAsync();
            return Ok(instalaciones);
        }

        // GET: api/instalaciones/{id}
        // The "read one" operation.
        [HttpGet("{id}")]
        public async Task<ActionResult<Instalacion>> GetInstalacion(int idinstalacion)
        { 
            var instalacion = await _contexto.Instalaciones.FindAsync(idinstalacion);

            if (instalacion == null)
            {
                return NotFound();
            }

            return Ok(instalacion);
        } // end of GetInstalacion method.

        // PUT: api/instalaciones/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarInstalacion(int idurl, Instalacion instalacion)
        {
            if (idurl != instalacion.InstalacionId)
            { 
                return BadRequest("El ID de la URL NO coincide con el ID de la instalación.");
            }

            _contexto.Entry(instalacion).State = EntityState.Modified;

            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_contexto.Instalaciones.Any(e => e.InstalacionId == idurl)) // no me autocompletó InstalacionId
                {
                    return NotFound();
                }
                throw;
            } // end of catch.

            return NoContent(); // 204 - actualizado exitósamente, nothing returned
        } // end of ActualizarInstalacion method.

        // DELETE: api/instalaciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarInstalacion(int idinstalacion)
        {
            var instalacionToDelete = await _contexto.Instalaciones.FindAsync(idinstalacion);

            if (instalacionToDelete == null)
            {
                return NotFound();
            }

            _contexto.Instalaciones.Remove(instalacionToDelete);
            await _contexto.SaveChangesAsync();

            return NoContent();
        } // end of EliminarInstalacion method.

    } // end of class InstalacionesController
}
