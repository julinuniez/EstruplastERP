using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    public partial class AgregaExtrusoraDestino : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🚀 IGNORAMOS LA CREACIÓN DE TABLAS Y SOLO AGREGAMOS LA COLUMNA
            migrationBuilder.AddColumn<string>(
                name: "ExtrusoraDestino",
                table: "Formulas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "UNICA");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 🚀 REVERTIMOS BORRANDO SOLO LA COLUMNA
            migrationBuilder.DropColumn(
                name: "ExtrusoraDestino",
                table: "Formulas");
        }
    }
}