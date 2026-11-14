using System.ComponentModel.DataAnnotations;
using DAS_Proyecto.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DAS_Proyecto.ViewModels
{
    public class CotizacionFormViewModel
    {
        public int IdCotizacion { get; set; }
        public string? CodigoCotizacion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente.")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona el usuario responsable.")]
        [Display(Name = "Usuario responsable")]
        public int IdUsuario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un servicio.")]
        [Display(Name = "Servicio")]
        public int IdServicio { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de cotización")]
        public DateTime FechaCotizacion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La descripción del trabajo es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        [Display(Name = "Descripción del trabajo")]
        public string DescripcionTrabajo { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "El costo de materiales no puede ser negativo.")]
        [Display(Name = "Costo de materiales")]
        public decimal CostoMateriales { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "El costo de mano de obra no puede ser negativo.")]
        [Display(Name = "Costo de mano de obra")]
        public decimal CostoManoObra { get; set; }

        [Display(Name = "Estado")]
        public EstadoCotizacion Estado { get; set; } = EstadoCotizacion.Pendiente;

        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Usuarios { get; set; } = new();
        public List<SelectListItem> Servicios { get; set; } = new();

        public decimal TotalEstimado => CostoMateriales + CostoManoObra;
    }
}
