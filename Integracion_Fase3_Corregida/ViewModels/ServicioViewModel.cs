using System.ComponentModel.DataAnnotations;

namespace DAS_Proyecto.ViewModels
{
    public class ServicioViewModel
    {
        public int IdServicio { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre del servicio")]
        [Display(Name = "Nombre del Servicio")]
        public string NombreServicio { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio base es obligatorio")]
        [Range(0.01, 1000000.00, ErrorMessage = "El precio debe ser mayor a cero")]
        [Display(Name = "Precio Base")]
        public decimal PrecioBase { get; set; }
    }
}
