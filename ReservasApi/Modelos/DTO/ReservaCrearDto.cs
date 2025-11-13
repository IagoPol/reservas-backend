namespace ReservasApi.Modelos.DTO
{
    public class ReservaCrearDto // this defines the fields Swagger will ask for when creating a reservation (y más importante, determina los que no pondré en swagger, estos son, los que NO pongo en esta clase).
    {
        // Primary key en EFC es por defecto autoincrementable al ser de tipo int.

        // Claves foráneas:
        public string Dni { get; set; } = string.Empty; // tal cual está en Reserva.cs
        public int InstalacionId { get; set; } // tal cual está en Reserva.cs

        // Campos "normales y corrientes" (i.e. ni son pk ni son fk)
        public DateTime FechaHora { get; set; } // tal cual en Reserva.cs. FechaHora es fecha de inicio de la reserva, me falta crear el campo de fecha fin o duración de la reserva.
        public int NumeroAsistentes { get; set; } // tal cual en Reserva.cs
    }
}
