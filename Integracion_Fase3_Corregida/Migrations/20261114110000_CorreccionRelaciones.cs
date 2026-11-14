using DAS_Proyecto.Data;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAS_Proyecto.Migrations
{
    public partial class CorreccionRelaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La migración AjusteModelo creó claves foráneas sombra además de los
            // Id... definidos en las entidades. Se conservan los Id... explícitos.
            migrationBuilder.Sql(@"
                UPDATE Cotizaciones SET IdCliente = ClienteIdCliente
                WHERE IdCliente = 0 AND ClienteIdCliente IS NOT NULL;
                UPDATE Cotizaciones SET IdUsuario = UsuarioIdUsuario
                WHERE IdUsuario = 0 AND UsuarioIdUsuario IS NOT NULL;
                UPDATE Cotizaciones SET IdServicio = ServicioIdServicio
                WHERE IdServicio = 0 AND ServicioIdServicio IS NOT NULL;
                UPDATE OrdenesTrabajo SET IdCotizacion = CotizacionIdCotizacion
                WHERE IdCotizacion = 0 AND CotizacionIdCotizacion IS NOT NULL;
                UPDATE DetalleOrdenMateriales SET IdOrdenTrabajo = OrdenTrabajoIdOrdenTrabajo
                WHERE IdOrdenTrabajo = 0 AND OrdenTrabajoIdOrdenTrabajo IS NOT NULL;
                UPDATE DetalleOrdenMateriales SET IdMaterial = MaterialIdMaterial
                WHERE IdMaterial = 0 AND MaterialIdMaterial IS NOT NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_Clientes_ClienteIdCliente",
                table: "Cotizaciones");
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_Servicios_ServicioIdServicio",
                table: "Cotizaciones");
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_Usuarios_UsuarioIdUsuario",
                table: "Cotizaciones");
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesTrabajo_Cotizaciones_CotizacionIdCotizacion",
                table: "OrdenesTrabajo");
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleOrdenMateriales_Materiales_MaterialIdMaterial",
                table: "DetalleOrdenMateriales");
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleOrdenMateriales_OrdenesTrabajo_OrdenTrabajoIdOrdenTrabajo",
                table: "DetalleOrdenMateriales");

            migrationBuilder.DropIndex(name: "IX_Cotizaciones_ClienteIdCliente", table: "Cotizaciones");
            migrationBuilder.DropIndex(name: "IX_Cotizaciones_ServicioIdServicio", table: "Cotizaciones");
            migrationBuilder.DropIndex(name: "IX_Cotizaciones_UsuarioIdUsuario", table: "Cotizaciones");
            migrationBuilder.DropIndex(name: "IX_OrdenesTrabajo_CotizacionIdCotizacion", table: "OrdenesTrabajo");
            migrationBuilder.DropIndex(name: "IX_DetalleOrdenMateriales_MaterialIdMaterial", table: "DetalleOrdenMateriales");
            migrationBuilder.DropIndex(name: "IX_DetalleOrdenMateriales_OrdenTrabajoIdOrdenTrabajo", table: "DetalleOrdenMateriales");

            migrationBuilder.DropColumn(name: "ClienteIdCliente", table: "Cotizaciones");
            migrationBuilder.DropColumn(name: "ServicioIdServicio", table: "Cotizaciones");
            migrationBuilder.DropColumn(name: "UsuarioIdUsuario", table: "Cotizaciones");
            migrationBuilder.DropColumn(name: "CotizacionIdCotizacion", table: "OrdenesTrabajo");
            migrationBuilder.DropColumn(name: "MaterialIdMaterial", table: "DetalleOrdenMateriales");
            migrationBuilder.DropColumn(name: "OrdenTrabajoIdOrdenTrabajo", table: "DetalleOrdenMateriales");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_IdCliente",
                table: "Cotizaciones",
                column: "IdCliente");
            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_IdServicio",
                table: "Cotizaciones",
                column: "IdServicio");
            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_IdUsuario",
                table: "Cotizaciones",
                column: "IdUsuario");
            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_IdCotizacion",
                table: "OrdenesTrabajo",
                column: "IdCotizacion");
            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenMateriales_IdMaterial",
                table: "DetalleOrdenMateriales",
                column: "IdMaterial");
            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenMateriales_IdOrdenTrabajo",
                table: "DetalleOrdenMateriales",
                column: "IdOrdenTrabajo");
            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdOrdenTrabajo",
                table: "Pagos",
                columns: new[] { "IdOrdenTrabajo", "FechaPago" });
            migrationBuilder.CreateIndex(
                name: "IX_Pagos_CodigoPago",
                table: "Pagos",
                column: "CodigoPago",
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_Pagos_AnticipoPorOrden",
                table: "Pagos",
                column: "IdOrdenTrabajo",
                unique: true,
                filter: "[TipoPago] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_Clientes_IdCliente",
                table: "Cotizaciones",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_Servicios_IdServicio",
                table: "Cotizaciones",
                column: "IdServicio",
                principalTable: "Servicios",
                principalColumn: "IdServicio",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_Usuarios_IdUsuario",
                table: "Cotizaciones",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesTrabajo_Cotizaciones_IdCotizacion",
                table: "OrdenesTrabajo",
                column: "IdCotizacion",
                principalTable: "Cotizaciones",
                principalColumn: "IdCotizacion",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DetalleOrdenMateriales_Materiales_IdMaterial",
                table: "DetalleOrdenMateriales",
                column: "IdMaterial",
                principalTable: "Materiales",
                principalColumn: "IdMaterial",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DetalleOrdenMateriales_OrdenesTrabajo_IdOrdenTrabajo",
                table: "DetalleOrdenMateriales",
                column: "IdOrdenTrabajo",
                principalTable: "OrdenesTrabajo",
                principalColumn: "IdOrdenTrabajo",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_OrdenesTrabajo_IdOrdenTrabajo",
                table: "Pagos",
                column: "IdOrdenTrabajo",
                principalTable: "OrdenesTrabajo",
                principalColumn: "IdOrdenTrabajo",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Pagos_OrdenesTrabajo_IdOrdenTrabajo", "Pagos");
            migrationBuilder.DropForeignKey("FK_DetalleOrdenMateriales_Materiales_IdMaterial", "DetalleOrdenMateriales");
            migrationBuilder.DropForeignKey("FK_DetalleOrdenMateriales_OrdenesTrabajo_IdOrdenTrabajo", "DetalleOrdenMateriales");
            migrationBuilder.DropForeignKey("FK_OrdenesTrabajo_Cotizaciones_IdCotizacion", "OrdenesTrabajo");
            migrationBuilder.DropForeignKey("FK_Cotizaciones_Clientes_IdCliente", "Cotizaciones");
            migrationBuilder.DropForeignKey("FK_Cotizaciones_Servicios_IdServicio", "Cotizaciones");
            migrationBuilder.DropForeignKey("FK_Cotizaciones_Usuarios_IdUsuario", "Cotizaciones");

            migrationBuilder.DropIndex("IX_Pagos_IdOrdenTrabajo", "Pagos");
            migrationBuilder.DropIndex("IX_Pagos_CodigoPago", "Pagos");
            migrationBuilder.DropIndex("IX_Pagos_AnticipoPorOrden", "Pagos");
            migrationBuilder.DropIndex("IX_DetalleOrdenMateriales_IdMaterial", "DetalleOrdenMateriales");
            migrationBuilder.DropIndex("IX_DetalleOrdenMateriales_IdOrdenTrabajo", "DetalleOrdenMateriales");
            migrationBuilder.DropIndex("IX_OrdenesTrabajo_IdCotizacion", "OrdenesTrabajo");
            migrationBuilder.DropIndex("IX_Cotizaciones_IdCliente", "Cotizaciones");
            migrationBuilder.DropIndex("IX_Cotizaciones_IdServicio", "Cotizaciones");
            migrationBuilder.DropIndex("IX_Cotizaciones_IdUsuario", "Cotizaciones");

            migrationBuilder.AddColumn<int>(name: "ClienteIdCliente", table: "Cotizaciones", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "ServicioIdServicio", table: "Cotizaciones", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "UsuarioIdUsuario", table: "Cotizaciones", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "CotizacionIdCotizacion", table: "OrdenesTrabajo", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "MaterialIdMaterial", table: "DetalleOrdenMateriales", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "OrdenTrabajoIdOrdenTrabajo", table: "DetalleOrdenMateriales", type: "int", nullable: false, defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE Cotizaciones SET ClienteIdCliente = IdCliente;
                UPDATE Cotizaciones SET UsuarioIdUsuario = IdUsuario;
                UPDATE Cotizaciones SET ServicioIdServicio = IdServicio;
                UPDATE OrdenesTrabajo SET CotizacionIdCotizacion = IdCotizacion;
                UPDATE DetalleOrdenMateriales SET MaterialIdMaterial = IdMaterial;
                UPDATE DetalleOrdenMateriales SET OrdenTrabajoIdOrdenTrabajo = IdOrdenTrabajo;");

            migrationBuilder.CreateIndex("IX_Cotizaciones_ClienteIdCliente", "Cotizaciones", "ClienteIdCliente");
            migrationBuilder.CreateIndex("IX_Cotizaciones_ServicioIdServicio", "Cotizaciones", "ServicioIdServicio");
            migrationBuilder.CreateIndex("IX_Cotizaciones_UsuarioIdUsuario", "Cotizaciones", "UsuarioIdUsuario");
            migrationBuilder.CreateIndex("IX_OrdenesTrabajo_CotizacionIdCotizacion", "OrdenesTrabajo", "CotizacionIdCotizacion");
            migrationBuilder.CreateIndex("IX_DetalleOrdenMateriales_MaterialIdMaterial", "DetalleOrdenMateriales", "MaterialIdMaterial");
            migrationBuilder.CreateIndex("IX_DetalleOrdenMateriales_OrdenTrabajoIdOrdenTrabajo", "DetalleOrdenMateriales", "OrdenTrabajoIdOrdenTrabajo");

            migrationBuilder.AddForeignKey("FK_Cotizaciones_Clientes_ClienteIdCliente", "Cotizaciones", "ClienteIdCliente", "Clientes", principalColumn: "IdCliente", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Cotizaciones_Servicios_ServicioIdServicio", "Cotizaciones", "ServicioIdServicio", "Servicios", principalColumn: "IdServicio", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Cotizaciones_Usuarios_UsuarioIdUsuario", "Cotizaciones", "UsuarioIdUsuario", "Usuarios", principalColumn: "IdUsuario", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_OrdenesTrabajo_Cotizaciones_CotizacionIdCotizacion", "OrdenesTrabajo", "CotizacionIdCotizacion", "Cotizaciones", principalColumn: "IdCotizacion", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_DetalleOrdenMateriales_Materiales_MaterialIdMaterial", "DetalleOrdenMateriales", "MaterialIdMaterial", "Materiales", principalColumn: "IdMaterial", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_DetalleOrdenMateriales_OrdenesTrabajo_OrdenTrabajoIdOrdenTrabajo", "DetalleOrdenMateriales", "OrdenTrabajoIdOrdenTrabajo", "OrdenesTrabajo", principalColumn: "IdOrdenTrabajo", onDelete: ReferentialAction.Cascade);
        }
    }
}
