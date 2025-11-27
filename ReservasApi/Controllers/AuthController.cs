using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservasApi.Data;
using ReservasApi.Modelos;
using ReservasApi.Modelos.DTO;
using ReservasApi.Servicios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text; // me autocompleta las 3 palabras

namespace ReservasApi.Controllers
{
    [ApiController] // me lo autocompleta, pertenece a Microsoft.AspNetCore.Mvc
    [Route("api/[controller]")] // Route me lo autocompleta. Microsoft.ANC.Mvc.RouteAttribute.
    public class AuthController : ControllerBase // ControllerBase me lo autocompleta y es de Microsoft.AspNetCore.Mvc.
    {
        // 3 propiedades privadas de solo lectura:
        private readonly AppDbContext _contexto;
        private readonly AuthService _servicioAutenticacion;
        private readonly IConfiguration _config; // IConfiguration es una interfaz de Microsoft.Extensions.Configuration, represents a set of key/value application configuration properties.

        // Constructor parametrizado con esas 3 propiedades:
        public AuthController(AppDbContext contexto, AuthService servicioAutenticacion, IConfiguration config)
        {
            _contexto = contexto;
            _servicioAutenticacion = servicioAutenticacion;
            _config = config;
        }

        // POST: api/auth/register
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(RegisterDto rdto)
        {
            // 1. Comprobar si el usuario ya existe (check if user already exists):
            // AnyAsync asíncronamente determina whether any element of a sequence satisfies a condition.
            var usuarioExiste = await _contexto.Usuarios.AnyAsync(u => u.Dni == rdto.Dni);
            if (usuarioExiste)
                return BadRequest("Ya existe un usuario con este DNI");

            // 2. Crear pw hash + salt con la función que para ello definimos en AuthService.cs:
            _servicioAutenticacion.CrearPwHash(rdto.Password, out byte[] hash, out byte[] salt);

            // 3. Create new Usuario entity i.e. crear un nuevo objeto de la clase Usuario.cs:
            var usuario = new Usuario
            {
                Dni = rdto.Dni,
                NombreCompleto = rdto.NombreCompleto,
                Email = rdto.Email,
                Telefono = rdto.Telefono,
                Domicilio = rdto.Domicilio,
                // fechas:
                FechaNacimiento = rdto.FechaNacimiento,
                FechaRegistro = DateTime.UtcNow, // returns an object whose value is the current UTC date and time.
                // hash y salt serán los recién creados con CrearPwHash.
                PasswordHash = hash,
                PasswordSalt = salt
            };

            // 4. Save to DB:
            _contexto.Usuarios.Add(usuario); // if POST /api/auth/register endpoint is implemented correctly, then calling it will insert a new user into your Usuarios table in SQL Server.
            await _contexto.SaveChangesAsync();

            // 5. retornamos el éxito de la operación de registro de usuario:
            return Ok("Usuario registrado exitósamente");
        } // final de función Registrar i.e. final de POST: api/auth/register

        // POST: api/auth/login
        [HttpPost("loguear")]
        public async Task<IActionResult> Loguear(LoginDto ldto)
        {
            // 1. Get user from DB:
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Dni == ldto.Dni);
            if (usuario == null)
                return Unauthorized("DNI y/o contraseña inválidos");

            // 2. Verificar contraseña: 
            // en AuthService.cs: public bool VerificarPwHash(string pw, byte[] hashGuardado, byte[] saltGuardado)
            bool esValida = _servicioAutenticacion.VerificarPwHash(ldto.Password, usuario.PasswordHash, usuario.PasswordSalt); // alerta: pwhash/pwsalt may be null here.
            if (!esValida) // si la pw NO es válida, entonces:
                return Unauthorized("DNI y/o contraseña inválidos"); // security best practice: never reveal which field failed.

            // 3. Generar el token JWT:
            string tokenCreado = CrearToken(usuario); // CrearToken se define aquí mismo i.e. en AuthController.cs

            return Ok(new
            {
                mensaje = "Login exitoso", // 'mensaje' does not come from anywhere -you invent it on the spot. It's simply a property name of an anonymous JSON object you are returning in the HTTP response.
                token = tokenCreado,
                usuario = new
                { 
                    usuario.Dni,
                    usuario.NombreCompleto,
                    usuario.Email,
                    usuario.Telefono,
                    usuario.Domicilio,
                    usuario.FechaNacimiento,
                    usuario.FechaRegistro
                }
            });
        } // final de la función Loguear

        // HELPER: para Crear JWT Token, definimos función privada con param Usuario y que retorna string:
        private string CrearToken(Usuario user)
        {
            var listaDeClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Dni),
                new Claim(ClaimTypes.Name,user.NombreCompleto)
            };

            var llave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]) // alerta: '_config' is not null here.
            );

            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha512);

            var nuevoTokenJwt = new JwtSecurityToken(
                claims: listaDeClaims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(nuevoTokenJwt);
        } // final de la función CrearToken.

    } // final de la public class AuthController
}
