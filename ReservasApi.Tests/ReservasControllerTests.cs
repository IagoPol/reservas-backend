using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Controllers;
using ReservasApi.Data;
using ReservasApi.Modelos;
using ReservasApi.Modelos.DTO;

// using System.Linq;
// using System.Text;

namespace ReservasApi.Tests
{
    public class ReservasControllerTests
    {
        /*
        // Helper method to create a new in-memory DbContext
        // Set up In-Memory AppDbContext for tests
        // Each test should use a fresh in-memory database to avoid state sharing between tests.
        private AppDbContext GetInMemoryDbContext() // le quité string dbNombre como parámetro
        { 
            var opciones = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // dbNombre can be unique per test ("TestDb1", "TestDb2") to isolate them.
                .Options;
            return new AppDbContext(opciones);
        } // todo este metodo privado aparece greyed out while not yet used anywhere
        */

        // Test GET all reservas
        [Fact] // each test method must have the [Fact] attribute for xUnit to recognize it.
        public async Task GetReservas_RetornaAllReservas() // the test method must be inside the class that is marked public class ReservasControllerTests.
        {
            // Arrange: use TestHelpers to get an in-memory DbContext
            using var contexto = TestHelpers.GetInMemoryDbContext(); // "TestDb1" // este método retorna un 'AppDbContext', que es el tipo de dato del parámetro de ReservasController constructor.

            // Seed Usuarios:
            // creo the related Usuario e Instalacion objects in the in-memory database before adding the Reserva:
            var usuario1 = new Usuario { Dni = "12349876F", NombreCompleto = "Martin Gomez", Email = "mgomez@gmail.com", Telefono = "988765432", Domicilio = "Calle Falsa 123", FechaNacimiento = new DateOnly(1990, 1, 1), FechaRegistro = DateTime.Now, PasswordHash = new byte[] { 1 }, PasswordSalt = new byte[] { 2 } };
            var usuario2 = new Usuario { Dni = "67895432C", NombreCompleto = "Jane Smith", Email = "jsmith@gmail.com", Telefono = "981735230", Domicilio = "Calle Real 321", FechaNacimiento = new DateOnly(1995, 10, 11), FechaRegistro = DateTime.Now, PasswordHash = new byte[] { 3 }, PasswordSalt = new byte[] { 4 } };
            contexto.Usuarios.AddRange(usuario1, usuario2);

            // Seed Instalaciones:
            var instalacion1 = new Instalacion { InstalacionId = 1, Localizacion = "Pabellon A", CapacidadMaxima = 50, PrecioHora = 20.0, Tipo = true };
            var instalacion2 = new Instalacion { InstalacionId = 2, Localizacion = "Pabellon B", CapacidadMaxima = 30, PrecioHora = 15.0, Tipo = false};
            contexto.Instalaciones.AddRange(instalacion1,instalacion2);

            // Seed Reservas:
            contexto.Reservas.AddRange(
                new Reserva
                { 
                    ReservaId = 1,
                    Dni = usuario1.Dni,
                    InstalacionId = instalacion1.InstalacionId,
                    NumeroAsistentes = 2,
                    FechaHora = new DateTime(2025, 12, 15, 10, 0, 0)
                },
                new Reserva
                { 
                    ReservaId = 2,
                    Dni = usuario2.Dni,
                    InstalacionId = instalacion2.InstalacionId,
                    NumeroAsistentes = 3,
                    FechaHora = new DateTime(2025, 12, 18, 15, 30, 0)
                }
            );
            // contexto.Reservas.Add(new Reserva { ReservaId = 1, Dni = "12349876F", InstalacionId = 1, NumeroAsistentes = 2, FechaHora = new DateTime(2025, 12, 15, 10, 00, 00) }); // según chatgpt no me hace falta poner FechaHora
            // contexto.Reservas.Add(new Reserva { ReservaId = 2, Dni = "67895432C", InstalacionId = 2, NumeroAsistentes = 3, FechaHora = new DateTime(2025, 12, 18, 15, 30, 00) });
            
            await contexto.SaveChangesAsync();

            // instancio ReservasController pasándole el 'contexto' que justo antes inicialicé con el retorno de GetInMemoryDbContext.
            var controlador = new ReservasController(contexto); // 'contexto0 es de tipo AppDbContext, que es el tipo del param del constructor de ReservasController

            // Act:
            var resultado = await controlador.GetReservas(); // GetReservas lo tengo definido en ReservasController.cs

            // Assert adaptado al mi código de GetReservas() :
            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result); // idem assert genérico
            var reservas = Assert.IsAssignableFrom<List<ReservaLeerDto>>(okResultado.Value); // cambio Reserva (assert genérico) por ReservaLeerDto
            
            // Assets:
            // Count:
            Assert.Equal(2, reservas.Count); // idem assert genérico.

            // Assert.Contains to verify each seeded Reserva exists:
            Assert.Contains(reservas, r => r.ReservaId == 1 && r.Dni == usuario1.Dni && r.InstalacionId == instalacion1.InstalacionId);
            Assert.Contains(reservas, r => r.ReservaId == 2 && r.Dni == usuario2.Dni && r.InstalacionId == instalacion2.InstalacionId);

        } // final de [FACT] GetReservas_RetornaAllReservas()

        // You can add more [Fact] methods here for POST, GET by ID, PUT, DELETE
    } // final de la clase
} // final del namespace
