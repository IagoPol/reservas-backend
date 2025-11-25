using ReservasApi.Data;
using System.Security.Cryptography;
using System.Text;

namespace ReservasApi.Servicios
{
    public class AuthService
    { // consta de una propiedad de tipo mi dbcontext, un constructor parametrizado con dicha propiedad y dos métodos (crearpwhash y verificarpw)
        // propiedad privada de solo lectura de tipo mi AppDbContext:
        private readonly AppDbContext _contexto;

        // constructor de la clase parametrizado con esa propiedad:
        public AuthService(AppDbContext c)
        {
            _contexto = c;
        }

        // Método público que no retorna nada y sirve para crear pw hash:
        public void CrearPwHash(string pw, out byte[] pwHash, out byte[] pwSalt)
        {
            using (var hmac = new HMACSHA512()) // EL HMA... me lo autocomplete el intellisense.
            {
                pwSalt = hmac.Key; // .Key me lo reconoce/recomienda el intellisense
                pwHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pw));
            }
        }

        // Método que retorna buleano y verifica la password:
        public bool VerificarPwHash(string pw, byte[] hashGuardado, byte[] saltGuardado)
        {
            using (var hmac = new HMACSHA512(saltGuardado))
            {
                var hashComputado = hmac.ComputeHash(Encoding.UTF8.GetBytes(pw));
                return hashComputado.SequenceEqual(hashGuardado);
            }
        } // final del método VerificarPwHash

    } // final de la public class AuthService, la cual uso en Program.cs en línea builder.Services.AddScoped<AuthService>();
} // namespace
