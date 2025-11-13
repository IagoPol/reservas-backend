using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasApi.Modelos
{
    public class Reserva
    {   // para cada campo empleo la sintaxis sintética propia de c sharp para getter y setter, fusiona varias líneas de código en una sola
        public int ReservaId { get; set; } // pk de tabla de reservas. No puse encima [Key] porque le llamé ReservaId; in EFC, you don't need the [Key] attribute if your property follows EF Core's naming conventions -and ReservaId does.
        // EF Core automatically recognizes ReservaId (or Id) as a primary key, so no extra attributes are required unless you prefer to be explicity (por ejemplo [Key]).
        public int NumeroAsistentes { get; set; } // cuánta gente asistirá al espacio reservado
        public DateTime FechaHora { get; set; } // fecha y hora de asistencia al espacio reserva; la fecha de inicio.
        // me falta poner FechaFin o duración de la reserva.

        // FK que referencia a PK de tabla de usuarios osease al dni:
        [Required] // esto exige poner: using System.ComponentModel.DataAnnotations;
        [ForeignKey("Usuario")] // esto exige poner: using System.ComponentModel.DataAnnotations.Schema;
        public string Dni { get; set; } = string.Empty; // FK -> pk (dni) de tabla de usuarios registrados

        // Propiedad de navegación:
        public Usuario Usuario { get; set; } = null!; // una reserva siempre es realizada por un solo usuario ni más ni menos (por ende NO pongo ?).
        // la exclamación en null! is the null-forgiving operator. It means "yes, compiler, I promise this property will not actually be null at runtime -even though I am initializing it with null for now". It silences the warning "non-nullable property 'Usuario' must contain non-null value when exiting constructor."
        // usamos null! porque EFCore does not actually assign "it" (a value, diría yo) right away when creating a new entity (al crear un nuevo objeto de la clase Usuario, diría yo)

        // fk que referencia a pk de tabla de instalaciones
        [Required]
        [ForeignKey("Instalacion")] // con esto tengo entendido que es suficiente para que EFC sepa que esta fk apunta a la pk de tabla instalaciones.
        public int InstalacionId { get; set; } // FK -> pk de tabla de instalaciones

        // Propiedad de navegación:
        public Instalacion Instalacion { get; set; } = null!; // una reserva siempre lo es sobre una instalación ni más ni menos (por ende no pongo ?).
        // la exclamación en null! is the null-forgiving operator. It means "yes, compiler, I promise this property will not actually be null at runtime -even though I am initializing it with null for now". It silences the warning "non-nullable property 'Instalacion' must contain non-null value when exiting constructor."

        // Relación (1:1) con Pago, propiedad de navegación:
        public Pago? Pago { get; set; } // Cada reserva tiene un pago asociado (no más) y de momento voy dejar abierta la posibilidad de que exista una reserva impagada, lo cual trato de transmitir con ?.
        
        // Relación (1:1) con Cancelacion, propiedad de navegación:
        public Cancelacion? Cancelacion { get; set; } // Cada reserva es cancelada por como máximo una cancelación (y evidentemente una reserva podrá no haber sido cancelada osease podrá estar asociada a cero cancelaciones y esto lo represento con ?)
    } // en relación a property names, podría ser necesaria una configuración en AppDbContext.
}
