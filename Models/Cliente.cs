using System.ComponentModel.DataAnnotations;

namespace DAS_FASE2.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100)]
        public string? Correo { get; set; }

        [StringLength(150)]
        public string? Empresa { get; set; }

        [StringLength(250)]
        public string? Direccion { get; set; }

        public string? Observaciones { get; set; }
    }
}