using System.ComponentModel.DataAnnotations;

namespace DAS_Proyecto.ViewModels
{
    public class ClienteViewModel
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre del cliente")]
        [Display(Name = "Nombre completo / Contacto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono de contacto es necesario")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string? Correo { get; set; }

        public string? Empresa { get; set; }
        public string? Direccion { get; set; }
        public string? Observaciones { get; set; }
    }
}