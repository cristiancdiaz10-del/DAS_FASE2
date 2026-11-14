
using System;
using System.ComponentModel.DataAnnotations;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.ViewModels
{
    public class RegistrarPagoViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar una orden.")]
        public int IdOrdenTrabajo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de pago.")]
        public TipoPago TipoPago { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(typeof(decimal), "0.01", "999999999999.99",
            ErrorMessage = "El monto debe ser mayor que cero.")]
        public decimal MontoPago { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; } = DateTime.Today;
    }
}
