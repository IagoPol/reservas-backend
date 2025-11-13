// DateTime viene de un using System que parece ser que no tengo necesidad de poner

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasApi.Modelos
{
    public class Cancelacion
    {
        public int CancelacionId { get; set; } // clave primaria de tabla de cancelaciones

        // FK que apunta a pk de tabla de reservas:
        [Required] // using System.ComponentModel.DataAnnotations
        [ForeignKey("Reserva")] // con esto EFC entiende que esta fk apunta a pk de tabla de reservas.
        public int ReservaId { get; set; } // fk -> pk de tabla de reservas

        // Propiedad de navegación: cada cancelación siempre cancela una sola reserva, ni más ni menos (1,1):
        public Reserva Reserva { get; set; } = null!; // una cancelación cancela siempre una sola reserva ni más ni menos (por ende NO pongo ?).
        // null! para que no aparezca la advertencia "El elemento propiedad "Reserva" que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador 'required' o declarar el propiedad como un valor que acepta valores NULL." al hacer dotnet build.
        public DateTime Fecha { get; set; } // fecha de la cancelación de la reserva
        public string Motivo { get; set; } = string.Empty; // motivo o razón de la cancelación, podría ser un ENUM si se me ocurren las opciones posibles.
    }
}