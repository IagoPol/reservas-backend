using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservasApi.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instalaciones",
                columns: table => new
                {
                    InstalacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Localizacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapacidadMaxima = table.Column<int>(type: "int", nullable: false),
                    PrecioHora = table.Column<double>(type: "float", nullable: false),
                    Tipo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instalaciones", x => x.InstalacionId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Dni = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domicilio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Contrasinal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Dni);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    ReservaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroAsistentes = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InstalacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.ReservaId);
                    table.ForeignKey(
                        name: "FK_Reservas_Instalaciones_InstalacionId",
                        column: x => x.InstalacionId,
                        principalTable: "Instalaciones",
                        principalColumn: "InstalacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_Dni",
                        column: x => x.Dni,
                        principalTable: "Usuarios",
                        principalColumn: "Dni",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cancelaciones",
                columns: table => new
                {
                    CancelacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancelaciones", x => x.CancelacionId);
                    table.ForeignKey(
                        name: "FK_Cancelaciones_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "ReservaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    PagoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<double>(type: "float", nullable: false),
                    Metodo = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.PagoId);
                    table.ForeignKey(
                        name: "FK_Pagos_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "ReservaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cancelaciones_ReservaId",
                table: "Cancelaciones",
                column: "ReservaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ReservaId",
                table: "Pagos",
                column: "ReservaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_Dni",
                table: "Reservas",
                column: "Dni");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_InstalacionId",
                table: "Reservas",
                column: "InstalacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cancelaciones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Instalaciones");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
