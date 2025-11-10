using Microsoft.EntityFrameworkCore;
using ReservasApi.Modelos;


namespace ReservasApi.Data
{
    public class AppDbContext : DbContext // represents the bridge between my C# models and my SQL Server database. ETF uses it (esta clase) to create tables based on your models, query and save data, manage relationships (like foreign keys).
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) // Constructor - EF Core passes the configuration here
            : base(options) { }

        // Cada DbSet se corresponde con una model class (definida en un .cs file) y con una tabla en mi SQL Server database.
        // El primer nombre (el que está entre <>) es el nombre de la model class (del .cs file), mientras que el segundo nombre (el que puse en plural) será si no me confundo el nombre de la tabla en la base de datos.
        public DbSet<Reserva> Reservas { get; set; } // Reserva es mi c# class (the model, Reserva.cs). 'Reservas' is my DbSet property name, which EFC uses to represent a table in the database. So EFC will create a table called 'Reservas' in my SQL Server database, where each row will correspond to a 'Reserva' object.
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Instalacion> Instalaciones { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Cancelacion> Cancelaciones { get; set; }
    }
}
