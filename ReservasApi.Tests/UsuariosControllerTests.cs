using Microsoft.AspNetCore.Mvc;
using ReservasApi.Controllers;
using ReservasApi.Modelos;

namespace ReservasApi.Tests
{
    public class UsuariosControllerTests
    {
        [Fact]
        public async Task GetUsuarios_RetornaAllUsuarios()
        {
            // Arrange
            using var contexto = TestHelpers.GetInMemoryDbContext();

            contexto.Usuarios.AddRange(
                new Usuario
                {
                    Dni = "34561278C",
                    NombreCompleto = "Marcos Gomez",
                    Email = "mgomez@gmail.com",
                    Telefono = "734985638",
                    Domicilio = "Calle Ventana 4",
                    FechaNacimiento = new DateOnly(1993, 03, 18),
                    FechaRegistro = DateTime.UtcNow,
                    PasswordHash = new byte[] { 1, 2, 3 },
                    PasswordSalt = new byte[] { 4, 5, 6 }
                },
                new Usuario
                {
                    Dni = "40901535L",
                    NombreCompleto = "Luis Diaz",
                    Email = "ldiaz@gmail.com",
                    Telefono = "884325790",
                    Domicilio = "Calle Rotulador 4",
                    FechaNacimiento = new DateOnly(1997, 05, 20),
                    FechaRegistro = DateTime.UtcNow,
                    PasswordHash = new byte[] { 7, 8, 9 },
                    PasswordSalt = new byte[] { 10, 11, 12 }
                }
            ); // fin del AddRange

            await contexto.SaveChangesAsync();

            var controlador = new UsuariosController(contexto);

            // Act:
            var resultado = await controlador.GetUsuarios();

            // Assert:
            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
            var usuarios = Assert.IsAssignableFrom<IEnumerable<Usuario>>(okResultado.Value);

            Assert.Equal(2, usuarios.Count()); // to verifiy the number of users returned.
            // To verify their actual content:
            Assert.Contains(usuarios, u => u.Dni == "34561278C");
            Assert.Contains(usuarios, u => u.Dni == "40901535L");
        } // fin de GetUsuarios_RetornaAllUsuarios() method.

    } // fin de la clase
} // end of namespace
