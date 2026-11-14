using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS_Proyecto.Models
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoPago { get; set; } = string.Empty;

        [Required]
        public int IdOrdenTrabajo { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoPago { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        public TipoPago TipoPago { get; set; }
    }
}