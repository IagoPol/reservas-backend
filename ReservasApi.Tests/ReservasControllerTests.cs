using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Controllers;
using ReservasApi.Data;
using ReservasApi.Modelos;
using ReservasApi.Modelos.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

// using System.Linq;
// using System.Text;

namespace ReservasApi.Tests
{
    public class ReservasControllerTests
    {
        // Helper method to create a new in-memory DbContext
        // Set up In-Memory AppDbContext for tests
        // Each test should use a fresh in-memory database to avoid state sharing between tests.
        private AppDbContext GetInMemoryDbContext(string dbNombre) {
            var opciones = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbNombre) // dbNombre can be unique per test ("TestDb1", "TestDb2") to isolate them.
                .Options;
            return new AppDbContext(opciones);
        } // todo este metodo privado aparece greyed out while not yet used anywhere

        // Test GET all reservas
        [Fact] // each test method must have the [Fact] attribute for xUnit to recognize it.
        public async Task GetReservas_RetornaAllReservas() // the test method must be inside the class that is marked public class ReservasControllerTests.
        {
            // Arrange:
            using var contexto = GetInMemoryDbContext("TestDb1"); // este método retorna un 'AppDbContext', que es el tipo de dato del parámetro de ReservasController constructor.
            contexto.Reservas.Add(new Reserva { ReservaId = 1, Dni = "12349876F", InstalacionId = 1, NumeroAsistentes = 2 }); // según chatgpt no me hace falta poner FechaHora
            contexto.Reservas.Add(new Reserva { ReservaId = 2, Dni = "67895432C", InstalacionId = 2, NumeroAsistentes = 3 });
            contexto.SaveChanges();

            // instancio ReservasController pasándole el 'contexto' que justo antes inicialicé con el retorno de GetInMemoryDbContext.
            var controlador = new ReservasController(contexto); // 'contexto0 es de tipo AppDbContext, que es el tipo del param del constructor de ReservasController

            // Act:
            var resultado = await controlador.GetReservas(); // GetReservas lo tengo definido en ReservasController.cs

            // Assert adaptado al mi código de GetReservas() :
            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result); // idem assert genérico
            var reservas = Assert.IsAssignableFrom<List<ReservaLeerDto>>(okResultado.Value); // cambio Reserva (assert genérico) por ReservaLeerDto
            Assert.Equal(2, reservas.Count); // idem assert genérico.
            /*
            // Assert genérico (used when a controller returns entities directly):
            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
            var reservas = Assert.IsAssignableFrom<List<Reserva>>(okResultado.Value);
            Assert.Equal(2, reservas.Count);
            */
        } // final de [FACT] GetReservas_RetornaAllReservas()

        // You can add more [Fact] methods here for POST, GET by ID, PUT, DELETE
    }
}
