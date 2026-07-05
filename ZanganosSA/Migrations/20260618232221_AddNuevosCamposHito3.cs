using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZanganosSA.Migrations
{
    /// <inheritdoc />
    public partial class AddNuevosCamposHito3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostoMantenimiento",
                table: "Colmenas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaProximaAlimentacion",
                table: "Colmenas",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoMantenimiento",
                table: "Colmenas");

            migrationBuilder.DropColumn(
                name: "FechaProximaAlimentacion",
                table: "Colmenas");
        }
    }
}
