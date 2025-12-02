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
    public class PagosControllerTests
    {
        [Fact]
        public async Task GetPagos_RetornaAllPagos()
        { 
            // Arrange: In-memory DbContext
            using var c = TestHelpers.GetInMemoryDbContext();

            // Seed Usuario (por ser fk requerida en tabla de reservas y ser reserva la fk que tabla de pagos requiere):
            var user = new Usuario
            {
                Dni = "90876532R",
                NombreCompleto = "Matias Lendoiro",
                Email = "mlend@gmail.com",
                Telefono = "600123456",
                Domicilio = "Calle Luna 9",
                FechaNacimiento = new DateOnly(1985, 02, 15),
                FechaRegistro = DateTime.Now,
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 2 },
            };
            c.Usuarios.Add(user);

            // Seed Instalacion (tmb es necesario para que una reserva exista):
            var i = new Instalacion { InstalacionId = 1, Localizacion = "Pabellon Central", CapacidadMaxima = 100, PrecioHora = 20.0, Tipo = true };
            c.Instalaciones.Add(i);

            // Seed Reserva (se necesita para que el pago pueda existir i.e. reserva es fk requerida en tabla de pagos):
            var r = new Reserva { ReservaId = 2, Dni = user.Dni, InstalacionId = i.InstalacionId, NumeroAsistentes = 4, FechaHora = new DateTime(2025, 12, 20, 18, 00, 00) };
            c.Reservas.Add(r);

            // Seed un pago:
            var pago = new Pago
            {
                PagoId = 1,
                ReservaId = r.ReservaId,
                Monto = 50.0,
                Metodo = MetodoPago.Tarjeta,
                FechaHora = new DateTime(2025, 11, 29, 09, 00, 00)
            };
            c.Pagos.Add(pago);
            await c.SaveChangesAsync();

            // Creamos una instancia del controlador de pagos pasándole el contexto como param:
            var controller = new PagosController(c);
            // Act:
            var resultado = await controller.GetPagos();

            // Assert:
            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
            var pagos = Assert.IsAssignableFrom<List<Pago>>(okResultado.Value);

            // el conteo:
            Assert.Single(pagos); // should be exactly one pago
            // contenido:
            Assert.Equal(1, pagos[0].PagoId); // matches seeded pago
            Assert.Equal(2, pagos[0].ReservaId); // points to the correct reserva
            Assert.Equal(50.0, pagos[0].Monto);

            // Assert.Contains para verificar el Pago:
            Assert.Contains(pagos, p => p.PagoId == pago.PagoId && p.ReservaId == r.ReservaId && p.Monto == pago.Monto);
        } // end of GetPagos_RetornaAllPagos method

        [Fact]
        public async Task CrearPago_CreaNuevoPagoAndRetornaCreated()
        {
            // Arrange: in-memory context
            using var c = TestHelpers.GetInMemoryDbContext();

            // Seed Usuario (fk requerida por Reserva, que a su vez es fk requerida por Pago):
            var user = new Usuario
            {
                Dni = "11223344A",
                NombreCompleto = "Pepe Garcia",
                Email = "pepe@hotmail.com",
                Telefono = "722409080",
                Domicilio = "Calle Sonrisa 3, Vigo",
                FechaNacimiento = new DateOnly(1998, 03, 06),
                FechaRegistro = DateTime.Now,
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 2 },

            };
            c.Usuarios.Add(user);

            // Seed Instalacion (por la misma razón que seed Usuario):
            var inst = new Instalacion
            {
                InstalacionId = 10,
                Localizacion = "Calle Venecia 3, Barcelona",
                CapacidadMaxima = 30,
                PrecioHora = 12.5,
                Tipo = true
            };
            c.Instalaciones.Add(inst);

            // Seed Reserva (fk requerida por Pago):
            var r = new Reserva
            {
                ReservaId = 5,
                Dni = user.Dni,
                InstalacionId = inst.InstalacionId,
                NumeroAsistentes = 3,
                FechaHora = new DateTime(2025, 12, 10, 16, 0, 0)
            };
            c.Reservas.Add(r);

            await c.SaveChangesAsync();

            // Create the new payment request:
            var nuevoPago = new Pago
            {
                ReservaId = r.ReservaId,
                Monto = 40.0,
                Metodo = MetodoPago.Tarjeta,
                FechaHora = new DateTime(2025,12,10,16,0,5)
            };

            // Instanciamos el controlador pasándole el contexto:
            var controlador = new PagosController(c);

            // Act:
            var resultado = await controlador.CrearPago(nuevoPago);

            // Assert: check that my controller response is an HTTP 201 Created response and that...
            //... it (ese controller) did return a proper CreatedAtActionResult.
            var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            // Assert: check that the CreatedAtAction(...) call is pointing to the correct action name (GetPago)
            Assert.Equal(nameof(PagosController.GetPago), created.ActionName);

            // Assert: EF Core should assign a new autogenerated PagoId (>0)
            var pagoCreado = Assert.IsType<Pago>(created.Value);
            Assert.True(pagoCreado.PagoId>0);

            // Assert: verify that the Pago objecct I sent in the POST request is exactly the same as...
            //... the Pago object returned by the controller.
            Assert.Equal(nuevoPago.ReservaId, pagoCreado.ReservaId);
            Assert.Equal(nuevoPago.Monto, pagoCreado.Monto);
            Assert.Equal(nuevoPago.Metodo, pagoCreado.Metodo);
            Assert.Equal(nuevoPago.FechaHora, pagoCreado.FechaHora);

            // Finalmente, verify that it (el nuevo pago) was saved in the DB:
            var pagoEnDb = await c.Pagos.FindAsync(pagoCreado.PagoId);
            Assert.NotNull(pagoEnDb);
            Assert.Equal(40.0, pagoEnDb!.Monto); // ! to silence the incorrect warning "pagoEnDb might be null here"
        } // end of CrearPago_CreaNuevoPagoAndRetornaCreated

    } // end of class PagosControllerTests
}
