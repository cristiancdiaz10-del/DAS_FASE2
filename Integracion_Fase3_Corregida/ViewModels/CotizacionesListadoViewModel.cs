using DAS_Proyecto.Models;

namespace DAS_Proyecto.ViewModels
{
    public class CotizacionesListadoViewModel
    {
        public IReadOnlyList<Cotizacion> Cotizaciones { get; set; } = Array.Empty<Cotizacion>();
        public string? Busqueda { get; set; }
        public string? EstadoSeleccionado { get; set; }
    }
}
