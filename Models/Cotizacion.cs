using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Models
{
    public enum EstadoCotizacion
    {
        Pendiente,
        Aceptada,
        Rechazada
    }

    public class Cotizacion
    {
        [Key]
        public int IdCotizacion { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoCotizacion { get; set; } = string.Empty;

        // Relaciones
        [Required]
        public int IdCliente { get; set; }
        public Cliente Cliente { get; set; } = null!;

        [Required]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [Required]
        public int IdServicio { get; set; }
        public Servicio Servicio { get; set; } = null!;

        // Información de la cotización
        [Required]
        public DateTime FechaCotizacion { get; set; } = DateTime.Now;

        [Required]
        [StringLength(1000)]
        public string DescripcionTrabajo { get; set; } = string.Empty;

        // Costos
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999)]
        public decimal CostoMateriales { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999)]
        public decimal CostoManoObra { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCotizacion { get; private set; }

        // Estado
        public EstadoCotizacion Estado { get; set; } =
            EstadoCotizacion.Pendiente;

        // Cálculo del total
        public void CalcularTotal()
        {
            TotalCotizacion = CostoMateriales + CostoManoObra;
        }
    }
}
