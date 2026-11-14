using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAS_Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class AgregaTablaPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    MontoPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoPago = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IdPago);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagos");
        }
    }
}
