namespace ReservasApi.Modelos
{
    public class Usuario
    {
        public string UsuarioId { get; set; } = string.Empty; // el dni es la primary key referenciada por fk en tabla de reservas
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        // fechas:
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }
        // contraseña:
        public string Contrasinal { get; set; } = string.Empty;

        // propiedad de navegación: un usuario registrado puede haber realizado de cero a varias reservas (0,N)
        public ICollection<Reserva>? Reservas { get; set; } // la propiedad es nullable osease puede tener un valor null -> a user can exist without any Reserva entries, it's valid for a user to exist having zero bookings.
    }
}
