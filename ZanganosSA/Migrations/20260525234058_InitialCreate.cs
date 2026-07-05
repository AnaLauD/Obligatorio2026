using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZanganosSA.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UbicacionGps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeccionalPolicial = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apiarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colmenas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoGeneral = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoReina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UltimaInspeccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApiarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colmenas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Colmenas_Apiarios_ApiarioId",
                        column: x => x.ApiarioId,
                        principalTable: "Apiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cosechas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCosecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PesoTotalEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApiarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cosechas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cosechas_Apiarios_ApiarioId",
                        column: x => x.ApiarioId,
                        principalTable: "Apiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tratamientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Medicamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuracionDias = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColmenaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tratamientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tratamientos_Colmenas_ColmenaId",
                        column: x => x.ColmenaId,
                        principalTable: "Colmenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Barriles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoLote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PesoNeto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaEnvasado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CosechaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barriles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Barriles_Cosechas_CosechaId",
                        column: x => x.CosechaId,
                        principalTable: "Cosechas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColmenaCosechas",
                columns: table => new
                {
                    ColmenaId = table.Column<int>(type: "int", nullable: false),
                    CosechaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColmenaCosechas", x => new { x.ColmenaId, x.CosechaId });
                    table.ForeignKey(
                        name: "FK_ColmenaCosechas_Colmenas_ColmenaId",
                        column: x => x.ColmenaId,
                        principalTable: "Colmenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ColmenaCosechas_Cosechas_CosechaId",
                        column: x => x.CosechaId,
                        principalTable: "Cosechas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Barriles_CosechaId",
                table: "Barriles",
                column: "CosechaId");

            migrationBuilder.CreateIndex(
                name: "IX_ColmenaCosechas_CosechaId",
                table: "ColmenaCosechas",
                column: "CosechaId");

            migrationBuilder.CreateIndex(
                name: "IX_Colmenas_ApiarioId",
                table: "Colmenas",
                column: "ApiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cosechas_ApiarioId",
                table: "Cosechas",
                column: "ApiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tratamientos_ColmenaId",
                table: "Tratamientos",
                column: "ColmenaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barriles");

            migrationBuilder.DropTable(
                name: "ColmenaCosechas");

            migrationBuilder.DropTable(
                name: "Tratamientos");

            migrationBuilder.DropTable(
                name: "Cosechas");

            migrationBuilder.DropTable(
                name: "Colmenas");

            migrationBuilder.DropTable(
                name: "Apiarios");
        }
    }
}
