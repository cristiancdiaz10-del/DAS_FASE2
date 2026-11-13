
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS_Proyecto.Models
{
    public class DetalleOrdenMaterial
    {
        [Key]
        public int IdDetalleOrdenMaterial { get; set; }

        // Relación con OrdenTrabajo
        [Required]
        public int IdOrdenTrabajo { get; set; }

        public OrdenTrabajo OrdenTrabajo { get; set; } = null!;

        // Relación con Material
        [Required]
        public int IdMaterial { get; set; }

        public Material Material { get; set; } = null!;

        // Cantidad y precio del material utilizado
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal CantidadUtilizada { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; private set; }

        // Cálculo del subtotal
        public void CalcularSubtotal()
        {
            Subtotal = CantidadUtilizada * PrecioUnitario;
        }
    }
}
