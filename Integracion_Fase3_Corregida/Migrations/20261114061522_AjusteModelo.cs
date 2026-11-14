using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAS_Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class AjusteModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Empresa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreServicio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PrecioBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.IdServicio);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    IdCotizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoCotizacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    ClienteIdCliente = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdServicio = table.Column<int>(type: "int", nullable: false),
                    ServicioIdServicio = table.Column<int>(type: "int", nullable: false),
                    FechaCotizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DescripcionTrabajo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CostoMateriales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoManoObra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCotizacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.IdCotizacion);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Clientes_ClienteIdCliente",
                        column: x => x.ClienteIdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Servicios_ServicioIdServicio",
                        column: x => x.ServicioIdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajo",
                columns: table => new
                {
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoOrden = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdCotizacion = table.Column<int>(type: "int", nullable: false),
                    CotizacionIdCotizacion = table.Column<int>(type: "int", nullable: false),
                    FechaOrden = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntregaEstimada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEntregaReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnticipoRequerido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajo", x => x.IdOrdenTrabajo);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Cotizaciones_CotizacionIdCotizacion",
                        column: x => x.CotizacionIdCotizacion,
                        principalTable: "Cotizaciones",
                        principalColumn: "IdCotizacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleOrdenMateriales",
                columns: table => new
                {
                    IdDetalleOrdenMaterial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    OrdenTrabajoIdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    MaterialIdMaterial = table.Column<int>(type: "int", nullable: false),
                    CantidadUtilizada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleOrdenMateriales", x => x.IdDetalleOrdenMaterial);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenMateriales_Materiales_MaterialIdMaterial",
                        column: x => x.MaterialIdMaterial,
                        principalTable: "Materiales",
                        principalColumn: "IdMaterial",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenMateriales_OrdenesTrabajo_OrdenTrabajoIdOrdenTrabajo",
                        column: x => x.OrdenTrabajoIdOrdenTrabajo,
                        principalTable: "OrdenesTrabajo",
                        principalColumn: "IdOrdenTrabajo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClienteIdCliente",
                table: "Cotizaciones",
                column: "ClienteIdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ServicioIdServicio",
                table: "Cotizaciones",
                column: "ServicioIdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_UsuarioIdUsuario",
                table: "Cotizaciones",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenMateriales_MaterialIdMaterial",
                table: "DetalleOrdenMateriales",
                column: "MaterialIdMaterial");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenMateriales_OrdenTrabajoIdOrdenTrabajo",
                table: "DetalleOrdenMateriales",
                column: "OrdenTrabajoIdOrdenTrabajo");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_CotizacionIdCotizacion",
                table: "OrdenesTrabajo",
                column: "CotizacionIdCotizacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleOrdenMateriales");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajo");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
