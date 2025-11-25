namespace ReservasApi.Modelos.DTO
{
    public class RegisterDto
    {
        public string Dni { get; set; } = string.Empty; // igual que en Usuario.cs
        public string NombreCompleto { get; set; } = string.Empty; // igual que en Usuario.cs
        public string Email { get; set; } = string.Empty; // igual que en Usuario.cs
        public string Telefono { get; set; } = string.Empty; // igual que en Usuario.cs
        public string Domicilio { get; set; } = string.Empty; // igual que en Usuario.cs
        public DateOnly FechaNacimiento { get; set; } // igual que en Usuario.cs

        public string Password { get; set; } = string.Empty; // plain text from client
    } // final de la clase RegisterDto
}
