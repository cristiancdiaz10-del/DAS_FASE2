
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS_Proyecto.Models
{
    public enum EstadoOrden
    {
        PendienteAnticipo,
        Recepcion,
        EnProceso,
        Listo,
        Entregado
    }

    public class OrdenTrabajo
    {
        [Key]
        public int IdOrdenTrabajo { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoOrden { get; set; } = string.Empty;

        // Relación con la cotización
        [Required]
        public int IdCotizacion { get; set; }

        public Cotizacion Cotizacion { get; set; } = null!;

        // Fechas
        [Required]
        public DateTime FechaOrden { get; set; } = DateTime.Now;

        public DateTime? FechaEntregaEstimada { get; set; }

        public DateTime? FechaEntregaReal { get; set; }

        // Información del anticipo
        [Column(TypeName = "decimal(18,2)")]
        public decimal AnticipoRequerido { get; private set; }

        // Estado de la orden
        public EstadoOrden Estado { get; set; } =
            EstadoOrden.PendienteAnticipo;

        // Calcula el anticipo del 50 %
        public void CalcularAnticipo()
        {
            AnticipoRequerido = Cotizacion.TotalCotizacion * 0.50m;
        }
    }
}
