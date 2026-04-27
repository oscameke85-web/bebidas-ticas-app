using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BebidasTicasPedidos.Migrations
{
    public partial class AgregarActivoCliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: true // 🔥 importante: clientes existentes quedan activos
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Clientes");
        }
    }
}
