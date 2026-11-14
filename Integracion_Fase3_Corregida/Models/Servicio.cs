using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS_Proyecto.Models
{
    public class Servicio
    {
        [Key]
        public int IdServicio { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio")]
        [StringLength(100)]
        public string NombreServicio { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio base es obligatorio")]
        [Range(0.01, 1000000.00, ErrorMessage = "El precio debe ser mayor a cero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioBase { get; set; }
    }
}
