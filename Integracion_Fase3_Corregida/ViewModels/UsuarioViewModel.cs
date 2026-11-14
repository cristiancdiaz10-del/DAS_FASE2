using System.ComponentModel.DataAnnotations;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.ViewModels;

public class UsuarioViewModel
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "Debe ingresar el nombre del usuario.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(100)]
    public string Correo { get; set; } = string.Empty;

    // Obligatoria al crear; opcional al editar para conservar la contraseña actual.
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe contener al menos 8 caracteres.")]
    public string? Password { get; set; }

    public RolUsuario Rol { get; set; } = RolUsuario.Operativo;
}
