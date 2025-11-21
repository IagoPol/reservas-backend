/*
// COMENTO TODO ESTE CÓDIGO DE CARA A EMPEZAR CON TESTS UNITARIOS IGNORAR TODO LO DE KAFKA.
using Confluent.Kafka; // me lo reconoce/autocompleta post-hacer dotnet add package Confluent.Kafka estando situado en carpeta padre del .csproj file de mi proyecto backend.
using System.Text.Json;
using ReservasApi.Modelos; // para poder poner ": IServicioProductorKafa" sin error.

namespace ReservasApi.Eventos // o ReservasApi.Services o ReservasApi.Messages
{
    public class ProductorKafka : IServicioProductorKafka // KafkaProducerService.cs
    { // clase ProductorKafka debe implementar el miembro de la interfaz, dicho miembro es el método PublicarReservaCreadaAsync, el cual se define abstractamente en dicha interfaz.
        // Le defino dos propiedades privadas de solo lectura a esta clase:
        private readonly IProducer<Null, string> _productor;
        private readonly string _topic = "reservas.creadas";

        // Defino constructor no parametrizado de esta clase:
        public ProductorKafka()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092" // mi broker address, el cual procede del comando largo que ejecuté to run the kafka docker hub image y para crear el kafka docker container.
            }; // cambio localhost por kafka-broker

            _productor = new ProducerBuilder<Null, string>(config).Build();
        }

        // Ahora sí, implemento el miembro de la interfaz osease su método:
        public async Task PublicarReservaCreadaAsync(Reserva reserva) // producer method
        {
            // Serialize using System.Text.Json, esto se escribe/hace diferente si empleamos using Newtonsoft.Json; pondríamos JsonConvert.SerializeObject(new etc.
            var mensaje = JsonSerializer.Serialize(new
            {
                reserva.ReservaId,
                reserva.Dni,
                reserva.InstalacionId,
                reserva.FechaHora,
                reserva.NumeroAsistentes
            });

            // Log before publishing (esto lo añado en el proceso de intentar solucionar el problema de que el consumidor kafka no muestra json messages upon swagger posting):
            Console.WriteLine($"Publicando reserva {reserva.ReservaId} to topic {_topic} con mensaje: {mensaje}");


            try // esto lo añado en el proceso de intentar solucionar el problema de que el consumidor kafka no muestra json messages upon swagger posting
            {
                // Produce the message to Kafka
                var resultado = await _productor.ProduceAsync(
                _topic,
                new Message<Null, string> { Value = mensaje }
                );

                //Log success
                Console.WriteLine($"Kafka OK => {resultado.TopicPartitionOffset}");
            }
            catch (Exception ex)
            {
                // Log any errors:
                Console.WriteLine($"Error publicando reserva {reserva.ReservaId}: {ex.Message}");
            }
        } // final del body de public async Task PublicarReservaCreadaAsync

    } // final de la clase ProductorKafka.
}
*/