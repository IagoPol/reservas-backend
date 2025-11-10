namespace ReservasApi.Modelos
{
    public class Reserva
    {   // para cada campo empleo la sintaxis sintética propia de c sharp para getter y setter, fusiona varias líneas de código en una sola
        public int ReservaId { get; set; } // pk de tabla de reservas
        // EF Core automatically recognizes ReservaId (or Id) as a primary key, so no extra attributes are required unless you prefer to be explicity (por ejemplo [Key]).
        public int NumeroAsistentes { get; set; } // cuánta gente asistirá al espacio reservado
        public DateTime FechaHora { get; set; } // fecha y hora de asistencia al espacio reserva
        public string Dni { get; set; } = string.Empty; // FK -> pk de tabla de usuarios registrados
        public int EspacioId { get; set; } // FK -> pk de tabla de instalaciones
    } // en relación a property names, podría ser necesaria una configuración en AppDbContext.
}
