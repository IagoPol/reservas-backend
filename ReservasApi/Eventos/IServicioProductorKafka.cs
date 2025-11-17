using ReservasApi.Modelos;

namespace ReservasApi.Eventos
{ // esta interfaz la va a implementar mi clase ProductorKafka.
    public interface IServicioProductorKafka // IKafkaProducerService
    {
        Task PublicarReservaCreadaAsync(Reserva reserva); // en la implementación precedo esto de public async
    }
}
