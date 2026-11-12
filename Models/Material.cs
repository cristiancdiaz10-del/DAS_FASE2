using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS_Proyecto.Models
{
    public class Material
    {
        [Key]
        public int IdMaterial { get; set; }

        [Required]
        public string CodigoMaterial { get; set; } = string.Empty;

        [Required]
        public string NombreMaterial { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadDisponible { get; set; }

        [Required]
        public string UnidadMedida { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        public string? Descripcion { get; set; }
    }
}