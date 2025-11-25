using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservasApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contrasinal",
                table: "Usuarios");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Usuarios",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Usuarios",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "Contrasinal",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
