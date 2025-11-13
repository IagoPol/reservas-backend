// DateTime viene de un using System que parece ser que no tengo necesidad de poner

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasApi.Modelos
{
    public class Pago
    {
        public int PagoId { get; set; } // clave primaria de la tabla de pagos, al llamarse PagoId no debería tener que poner [Key] encima
        
        // FK que apunta a pk de tabla de reservas:
        [Required] // using System.ComponentModel.DataAnnotations
        [ForeignKey("Reserva")] // con esto EFC entiende que esta fk apunta a pk de tabla de reservas.
        public int ReservaId { get; set; } // fk -> pk de tabla de reservas

        // Propiedad de navegación: cada pago siempre paga una sola reserva, ni más ni menos y es por esto que NO pongo ?.
        public Reserva Reserva { get; set; } = null!; // para que no aparezca la advertencia de "El elemento propiedad "Reserva" que NO acepta valores NULL debe contener un valor DISTINTO de NULL al salir del constructor. Considere la posibilidad de agregar el modificador 'required' o declarar la propiedad como un valor que acepta valores NULL." al hacer dotnet build.

        public double Monto { get; set; } // cuánto paga el usuario en total por la reserva
        public MetodoPago Metodo { get; set; } // MetodoPago es una ENUM class donde cada instancia es un método de pago (por ejemplo CONTARJETA).
        public DateTime FechaHora { get; set; } // fecha y hora del pago (¿coincidirá con la fecha y hora en que se realizó la solicitud de reserva?).
    }
}