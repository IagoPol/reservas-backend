namespace ReservasApi.Modelos.DTO
{
    public class LoginDto
    {
        public string Dni { get; set; } = string.Empty; // igual que en Usuario.cs
        public string Password { get; set; } = string.Empty;
    }
}
