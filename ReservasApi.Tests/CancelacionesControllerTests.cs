using Microsoft.AspNetCore.Mvc;
using ReservasApi.Controllers;
using ReservasApi.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasApi.Tests
{
    public class CancelacionesControllerTests
    {
        [Fact]
        public async Task GetCancelaciones_RetornaAllCancelaciones()
        {
            // Arrange:
            using var c = TestHelpers.GetInMemoryDbContext();

            // Seed Usuario (requerido para Reserva, que a se vez es requerida para Cancelacion):
            var u = new Usuario
            {
                Dni = "15436798P",
                NombreCompleto = "Carlos Ruiz",
                Email = "cruiz@hotmail.com",
                Telefono = "722809070",
                Domicilio = "Calle Sol 8",
                FechaNacimiento = new DateOnly(1998, 04, 12),
                FechaRegistro = DateTime.UtcNow,
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 2 }
            };
            c.Usuarios.Add(u);

            // Seed Instalacion (requerido para la Reserva, que a su vez es requerida para Cancelacion):
            var inst = new Instalacion{InstalacionId = 5, Localizacion = "Bernabeu", CapacidadMaxima = 40, PrecioHora = 12.5, Tipo = true};
            c.Instalaciones.Add(inst);

            // Seed Reserva (requerida para la Cancelacion i.e. reserva es fk requerida de tabla de cancelaciones):
            var r = new Reserva{ReservaId = 3, Dni = u.Dni, InstalacionId = inst.InstalacionId, NumeroAsistentes = 3, FechaHora = new DateTime(2025, 12, 20, 17, 30, 00)};
            c.Reservas.Add(r);

            // Seed Cancelacion:
            var cancelacion = new Cancelacion { CancelacionId = 1, ReservaId = r.ReservaId, Fecha = new DateTime(2025, 12, 15, 18, 00, 00), Motivo = "Cambio de planes" };
            c.Cancelaciones.Add(cancelacion);

            await c.SaveChangesAsync();

            var controlador = new CancelacionesController(c);

            // Act:
            var resultado = await controlador.GetCancelaciones();

            // Assert:
            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
            var cancelaciones = Assert.IsAssignableFrom<List<Cancelacion>>(okResultado.Value);

            Assert.Single(cancelaciones);
            
            var anulacion = cancelaciones[0];

            // Verificación de basic values:
            Assert.Equal(1, anulacion.CancelacionId);
            Assert.Equal(3, anulacion.ReservaId);
            Assert.Equal("Cambio de planes", anulacion.Motivo);

        } // end of GetCancelaciones_RetornaAllCancelaciones method

        // POST /api/cancelaciones Success Test only osease only escenario de when reserva exists.
        [Fact]
        public async Task CrearCancelacion_RetornaCreated() // escenario de when reserva exists, faltaría añadir el escenario contrario.
        {
            // Arrange:
            using var contexto = TestHelpers.GetInMemoryDbContext();

            // Seed Usuario (requerido para Reserva, que a su vez es requerido para Cancelacion):
            var u = new Usuario
            { 
                Dni = "80765432C",
                NombreCompleto = "Carlos Ruiz",
                Email = "cruiz@gmail.com",
                Telefono = "740567890",
                Domicilio = "Calle Neptuno 10, Vigo",
                FechaNacimiento = new DateOnly(1970,07,29),
                FechaRegistro = DateTime.Now,
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 2 },
            };
            contexto.Usuarios.Add(u);

            // Seed Instalacion (requerido para Reserva que a su vez es requerido para Cancelacion):
            var inst = new Instalacion
            {
                InstalacionId = 5,
                Localizacion = "Camp Nou",
                CapacidadMaxima = 40,
                PrecioHora = 20.25,
                Tipo = true
            };
            contexto.Instalaciones.Add(inst);

            // Seed Reserva (requerido para Cancelacion)
            var r = new Reserva
            {
                ReservaId = 3,
                Dni = u.Dni,
                InstalacionId = inst.InstalacionId,
                NumeroAsistentes = 9,
                FechaHora = new DateTime(2025, 12, 13, 17, 00, 00)
            };
            contexto.Reservas.Add(r);

            await contexto.SaveChangesAsync();

            var controlador = new CancelacionesController(contexto);

            var nuevaCancelacion = new Cancelacion
            {
                ReservaId = r.ReservaId,
                Fecha = new DateTime(2025, 12, 10, 16, 00, 00),
                Motivo = "No puedo asistir"
            };

            // Act:
            var resultado = await controlador.CrearCancelacion(nuevaCancelacion);

            // Assert:
            var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var cancelacionCreada = Assert.IsType<Cancelacion>(created.Value);

            // Property checks:

        }
    } // end of public class CancelacionesControllerTests class
}
