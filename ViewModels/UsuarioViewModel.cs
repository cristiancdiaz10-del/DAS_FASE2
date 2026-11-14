using System.ComponentModel.DataAnnotations;
using DAS_FASE2.Models;

namespace DAS_FASE2.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre del usuario")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; } = RolUsuario.Operativo;
    }
}
