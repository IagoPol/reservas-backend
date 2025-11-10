namespace ReservasApi.Modelos
{
    public class Instalacion // cada objeto será o bien un espacio deportivo o bien una sala de estudio.
    {
        public int InstalacionId { get; set; } // pk referenciada por una fk de tabla de reservas
        public string Localizacion { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public double PrecioHora { get; set; }
        public bool Tipo { get; set; } // true será espacio deportivo y false será sala de estudio.

        // Propiedad de navegación: Una instalación puede estar reservada de cero a varias veces:
        public ICollection<Reserva>? Reservas { get; set; } // cardinalidad mínima y máxima de entidad Instalacion respecto a Reserva es (0,N)
    }
}
