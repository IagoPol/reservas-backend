namespace ReservasApi.Modelos.DTO
{
    public class ReservaLeerDto
    {
        // primary key:
        public int ReservaId { get; set; } // esto (la pk de tabla de reservas) no lo puse en ReservaCrearDto.cs
        // normal fields:
        public DateTime FechaHora { get; set; } // tal cual en ReservaCrearDto.cs
        public int NumeroAsistentes { get; set; } // tal cual en ReservaCrearDto.cs
        // foreign keys:
        public string Dni { get; set; } = string.Empty; // tal cual en ReservaCrearDto.cs
        public int InstalacionId { get; set; } // tal cual en ReservaCrearDto.cs
        
        // Navigation objects (only basic info to avoid cycles):
        public UsuarioDto? Usuario { get; set; }
        public InstalacionDto? Instalacion { get; set; }

    }
}
