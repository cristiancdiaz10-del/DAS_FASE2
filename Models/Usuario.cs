using System.ComponentModel.DataAnnotations;

namespace DAS_FASE2.Models
{
    public enum RolUsuario
    {
        Administrador,
        Operativo
    }

    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo de acceso es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string PasswordHash { get; set; } = string.Empty; // Nunca texto plano

        public RolUsuario Rol { get; set; } = RolUsuario.Operativo;
    }
}
