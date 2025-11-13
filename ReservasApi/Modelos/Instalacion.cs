namespace ReservasApi.Modelos
{
    public class Instalacion // cada objeto será o bien un espacio deportivo o bien una sala de estudio.
    {
        public int InstalacionId { get; set; } // pk referenciada por una fk de tabla de reservas. Es autoincrementable, NO se lo paso yo en Swagger al hacer el POST.
        public string Localizacion { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public double PrecioHora { get; set; }
        public bool Tipo { get; set; } // true será espacio deportivo y false será sala de estudio.

        // Propiedad de navegación: Una instalación puede estar reservada de cero a varias veces:
        public ICollection<Reserva>? Reservas { get; set; } // cardinalidad mínima y máxima de entidad Instalacion respecto a Reserva es (0,N), el cero lo represento con ? y la N con ICollection.
    }
}