
using System;
using System.Collections.Generic;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.ViewModels
{
    public class PagoListadoViewModel
    {
        // Información de las tarjetas
        public decimal TotalPagado { get; set; }

        public decimal TotalAnticipos { get; set; }

        public decimal TotalSaldosPendientes { get; set; }

        // Lista de pagos registrados
        public List<PagoItemViewModel> Pagos { get; set; }
            = new List<PagoItemViewModel>();
    }

    public class PagoItemViewModel
    {
        public string CodigoPago { get; set; } = string.Empty;

        public string CodigoOrden { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public decimal MontoPago { get; set; }

        public TipoPago TipoPago { get; set; }

        public DateTime FechaPago { get; set; }
    }
}
