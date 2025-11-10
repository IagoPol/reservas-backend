// DateTime viene de un using System que parece ser que no tengo necesidad de poner

namespace ReservasApi.Modelos
{
    public class Pago
    {
        public int PagoId { get; set; } // clave primaria de la tabla de pagos
        public int ReservaId { get; set; } // fk -> pk de tabla de reservas
        public double Monto { get; set; } // cuánto paga el usuario en total por la reserva
        public MetodoPago Metodo { get; set; } // MetodoPago es una ENUM class donde cada instancia es un método de pago (por ejemplo CONTARJETA).
        public DateTime FechaHora { get; set; } // fecha y hora del pago (¿coincidirá con la fecha y hora en que se realizó la solicitud de reserva?).

        // Propiedad de navegación: cada pago siempre paga una sola reserva, ni más ni menos osease (1,1) es la cardinalidad mínima y máxima de la entidad Pago respecto a la entidad Reserva.
        public Reserva? Reserva { get; set; }

    }
}
