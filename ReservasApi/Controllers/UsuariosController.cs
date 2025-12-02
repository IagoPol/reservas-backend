using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Modelos;

namespace ReservasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        // propiedad de tipo mi AppDbContext :
        private readonly AppDbContext _contexto;

        // constructor parametrizado con esa propiedad:
        public UsuariosController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // POST (Create), GET (Read), PUT (Updaet), DELETE (idem) respecto de la tabla de usuarios:
        // ESTE POST ESTÁ COMENTADO PORQUE YA NO ME HACE FALTA, YA QUE...
        // ... QUEDA REEMPLAZADO POR AUTHSERVICE, AUTHCONTROLLER Y REGISTERDTO.
        // El no haber puesto ese comentario me hizo perder hora y media el 1 dic de 8:00 a 9:30 intentando descrifrar por qué lo había comentado.
        /*
        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> CrearUsuario(Usuario nuevouser)
        {
            _contexto.Usuarios.Add(nuevouser);
            await _contexto.SaveChangesAsync();

            // CreatedAtAction automatically returns '201 Created' + Location header:
            return CreatedAtAction(nameof(GetUsuario), new { id = nuevouser.Dni }, nuevouser);
        } // método GetUsuario lo defino más abajo
        */

        // GET: api/usuarios
        // GetUsuarios tiene por propósito retornar una lista de todos los usuarios from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _contexto.Usuarios.ToListAsync(); // produces a List<Usuario>
            return Ok(usuarios); // usuarios is a List<Usuario>
        }

        // GET: api/usuarios/{id}
        // The "read one" operation.
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string docid)
        { 
            var usuario = await _contexto.Usuarios.FindAsync(docid);

            if (usuario == null)
            { 
                return NotFound();  
            }

            return Ok(usuario);
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(string idurl, Usuario usuario)
        {
            if (idurl != usuario.Dni)
            { 
                return BadRequest("El ID de la URL NO coincide con el ID del usuario");
            }

            _contexto.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_contexto.Usuarios.Any(e => e.Dni == idurl))
                {
                    return NotFound();
                }
                throw;
            } // end of catch

            return NoContent(); // 204 - actualizado exitósamente, no content returned.
        } // end of method ActualizarUsuario

        // DELETE: api/usuarios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(string docid)
        {
            var usuarioToDelete = await _contexto.Usuarios.FindAsync(docid);

            if (usuarioToDelete == null)
            {
                return NotFound();
            }

            _contexto.Usuarios.Remove(usuarioToDelete);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }

    } // end of public class UsuariosController
}
