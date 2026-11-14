
using System;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.ViewModels
{
    public class DetallePagoViewModel
    {
        // Información del pago
        public string CodigoPago { get; set; } = string.Empty;

        public TipoPago TipoPago { get; set; }

        public decimal MontoPago { get; set; }

        public DateTime FechaPago { get; set; }

        // Información de la orden asociada
        public string CodigoOrden { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public string EstadoOrden { get; set; } = string.Empty;
    }
}
