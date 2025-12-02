using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReservasApi.Controllers;
using ReservasApi.Modelos;
using ReservasApi.Modelos.DTO;
using ReservasApi.Servicios;

namespace ReservasApi.Tests
{
    public class UsuariosControllerTests
    {
        [Fact]
        public async Task GetUsuarios_RetornaAllUsuarios()
        {
            // Arrange: get in-memory DbContext
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

        [Fact]
        public async Task RegisterUser_RegistraNuevoUsuario()
        {
            // Arrange: get in-memory DbContext
            using var contexto = TestHelpers.GetInMemoryDbContext(); // contexto es de tipo AppDbContext ya que esto es el tipo de retorno de GetInMemoryDbContext.

            // AuthService class usa ese contexto
            // (AppDbContext es propiedad y constructor param de la clase AuthService y AppDbContext es precisamente el tipo de retorno de GetInMemoryDbContext)
            var servicioAutenticacion = new AuthService(contexto);

            // Mi clase AuthController como 3er param de su constructor tiene IConfiguration config, aunque...
            //... el método Registrar en sí never actually reads anything from the IConfiguration object.
            var config = new ConfigurationBuilder().Build();

            // Controller con dependencies (v. constructor en AuthController.cs)
            // Nótese que en test del get para inicializar el controlador usábamos UsuariosController en lugar de AuthController.
            // Por qué? Pues porque ese get está dentro de UsuariosController VS post /api/auth/registrar está en AuthController.cs
            var controlador = new AuthController(contexto, servicioAutenticacion, config);

            // RegisterDto para simular POST payLoad :
            var rd = new RegisterDto { Dni = "12345678A", NombreCompleto = "Matias Lopez", Email = "malo@gmail.com", Telefono = "981604050", Domicilio = "123 Street", FechaNacimiento = new DateOnly(1990, 05, 07), Password = "SecurePassword123!" };

            // Act: call the Registrar endpoint:
            var resultado = await controlador.Registrar(rd);

            // Assert: result is OkObjectResult:
            var okResultado = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal("Usuario registrado exitósamente", okResultado.Value);

            // Assert: user was actually added to the in-memory DB
            // aka user was actually saved in the database
            // aka assert that the user was acually persisted in the database.
            var userEnDb = await contexto.Usuarios.FirstOrDefaultAsync(u => u.Dni == rd.Dni);
            Assert.NotNull(userEnDb); // confirms that the Registrar method actually saved a user in the database.
            // Lo siguiente verifica that the saved user's properties match what was sent in the registration request
            // aka ensures that the controller did not just insert a blank user or mix up fields.
            Assert.Equal(rd.NombreCompleto, userEnDb.NombreCompleto);
            Assert.Equal(rd.Email, userEnDb.Email);

            // pwhash y pwsalt deberían existir:
            Assert.NotNull(userEnDb.PasswordHash);
            Assert.NotNull(userEnDb.PasswordSalt);
        }

    } // fin de la clase
} // end of namespace
