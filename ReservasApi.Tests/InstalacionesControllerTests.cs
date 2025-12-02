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
    public class InstalacionesControllerTests
    {
        [Fact]
        public async Task GetInstalaciones_RetornaAllInstalaciones()
        { 
            // Arrange: in-memory DbContext
            using var contexto = TestHelpers.GetInMemoryDbContext();

            // Sedd Instalaciones:
            var inst1 = new Instalacion { InstalacionId = 1, Localizacion = "Campo 5", CapacidadMaxima = 22, PrecioHora = 15.0, Tipo = true };
            var inst2 = new Instalacion { InstalacionId = 2, Localizacion = "Biblioteca 8", CapacidadMaxima = 50, PrecioHora = 8.0, Tipo = false };

            contexto.Instalaciones.AddRange(inst1, inst2);
            await contexto.SaveChangesAsync();

            // Instanciamos el controlador con el contexto:
            var controlador = new InstalacionesController(contexto);

            // Act:
            var resultado = await controlador.GetInstalaciones();

            // Assert:
            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
            var instalaciones = Assert.IsAssignableFrom<List<Instalacion>>(okResultado.Value);

            // test del conteo:
            Assert.Equal(2, instalaciones.Count);

            // test del contenido:
            Assert.Contains(instalaciones, i => i.InstalacionId == inst1.InstalacionId && i.Localizacion == inst1.Localizacion);
            Assert.Contains(instalaciones, i => i.InstalacionId == inst2.InstalacionId && i.Localizacion == inst2.Localizacion);
        } // final de GetInstalaciones_RetornaAllInstalaciones()

        [Fact]
        public async Task CrearInstalacion_AgregaInstalacionMasRetornaCreated()
        {
            // Arrange: in-memory DbContext:
            using var contexto = TestHelpers.GetInMemoryDbContext();

            // Creamos una instancia de Instaslacion:
            var nuevaVenue = new Instalacion { Localizacion = "Calle Vida 10, Vigo", CapacidadMaxima = 30, PrecioHora = 20.0, Tipo = true };

            // Instanciamos el controlador con el contexto:
            var controlador = new InstalacionesController(contexto);

            // Act:
            var resultado = await controlador.CrearInstalacion(nuevaVenue);

            // Assert: check response type
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);

            // Assert: check returned value aka check that the returned object contains the correct data
            var venueRetornada = Assert.IsType<Instalacion>(createdResult.Value);
            Assert.Equal(nuevaVenue.Localizacion, venueRetornada.Localizacion);
            Assert.Equal(nuevaVenue.CapacidadMaxima, venueRetornada.CapacidadMaxima);
            Assert.Equal(nuevaVenue.PrecioHora, venueRetornada.PrecioHora);
            Assert.Equal(nuevaVenue.Tipo, venueRetornada.Tipo);

            // Assert: check it (la instalación) was actually added to the DbContext
            var venueEnDb = await contexto.Instalaciones.FindAsync(venueRetornada.InstalacionId);
            Assert.NotNull(venueEnDb);
            Assert.Equal(nuevaVenue.Localizacion, venueEnDb.Localizacion);
            Assert.Equal(nuevaVenue.CapacidadMaxima, venueEnDb.CapacidadMaxima);
            Assert.Equal(nuevaVenue.PrecioHora, venueEnDb.PrecioHora);
            Assert.Equal(nuevaVenue.Tipo, venueEnDb.Tipo);

        }
    }
}
