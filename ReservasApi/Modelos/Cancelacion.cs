// DateTime viene de un using System que parece ser que no tengo necesidad de poner

namespace ReservasApi.Modelos
{
    public class Cancelacion
    {
        public int CancelacionId { get; set; } // clave primaria de tabla de cancelaciones
        public int ReservaId { get; set; } // fk -> pk de tabla de reservas
        public DateTime Fecha { get; set; } // fecha de la cancelación de la reserva
        public string Motivo { get; set; } = string.Empty; // motivo o razón de la cancelación, podría ser un ENUM si se me ocurren las opciones posibles.
        
        // Propiedad de navegación: cada cancelación siempre cancela una sola reserva, ni más ni menos (1,1):
        public Reserva? Reserva { get; set; }
    }
}