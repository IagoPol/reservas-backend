using System.ComponentModel.DataAnnotations;

namespace ReservasApi.Modelos
{
    public class Usuario
    {
        [Key]
        public string Dni { get; set; } = string.Empty; // el dni es la primary key referenciada por fk en tabla de reservas

        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;

        // fechas:
        public DateOnly FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }

        /*
        // contraseña as a raw string NO es segura:
        [Required] // makes the property required, so that it cannot be null or omitted when inserting/updating.
        public string Contrasinal { get; set; } = string.Empty; // this is "the property declaration", above it pones la 'Required' keyword.
        esta propiedad pasa para RegisterDto.cs y LoginDto.cs.
         */

        // Reemplazo la contraseña as raw string (no segura) por pwhash y pwsalt:
        [Required]
        public byte[]? PasswordHash { get; set; } // ? para evitar la alerta en output de dotnet build que dice "el elemento propiedad "pwhash" que NO acepta valores NULL debe contener un valor distinto de NULL al salir del constructor (mi Usuario class has NO constructor that sets them)
        [Required]
        public byte[]? PasswordSalt { get; set; }

        // propiedad de navegación: un usuario realiza entre 0 y varias reservas
        public ICollection<Reserva>? Reservas { get; set; } // el cero lo represento con ? y el varios con ICollection.
    } // final de public class Usuario
}
