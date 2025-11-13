using Confluent.Kafka; // me lo reconoce/autocompleta post-hacer dotnet add package Confluent.Kafka estando situado en carpeta padre del .csproj file de mi proyecto backend.
using System.Text.Json;

namespace ReservasApi.Eventos
{
    public class ProductorKafka
    {
        // propiedad privada de solo lecutra de tipo string
        private readonly string _servidoresBootstrap;

        // constructor parametrizado con esa propiedad
        public ProductorKafka(string servidoresBootstrap)
        { 
            _servidoresBootstrap = servidoresBootstrap;
        }

        // el param 'reserva' es the data that will be published to Kafka, 'object' means it can accept any type, pero en mi caso será Reserva model (mi booking entity).
        // Task indica que el método retorna una Task i.e. a "promise" that some work will complete in the future.
        // Since it's just Task (not Task<T>), that means it (the method) returns no actual value (it is like a void function, but asynchronous).
        public async Task PublicarReservaCreadaAsync(object reserva) // method signature, la acción es "publish a ReservaCreada EVENT asynchronously", es una operación asíncrona.
        {
            var config = new ProducerConfig // me lo reconoce/autcompleta gracias al using Confluent.Kafka
            {
                BootstrapServers = _servidoresBootstrap // BootstrapServer me lo reconoce/autocompleta
            };

            using var productor = new ProducerBuilder<Null, string>(config).Build();

            var mensaje = JsonSerializer.Serialize(reserva); // JsonSerializer y Serialize me los reconoce/autocompleta.

            // 'reservas.creadas' es el topic name from the schema I was given
            await productor.ProduceAsync("reservas.creadas", new Message<Null, string> { Value = mensaje }); // ProduceAsync y Value me los reconoce/autocompleta

            productor.Flush(TimeSpan.FromSeconds(5)); // Flush, TimeSpan y FromSeconds me los reconoce/autocompleta.
        } // final del body del método PublicarReservaCreadaAsync

    } // final de la clase ProductorKafka.
}