using System;
using System.Collections.Generic;

namespace DAS_Proyecto.ViewModels
{
    public class InformeMensualViewModel
    {
        // ============================================
        // FILTRO DEL INFORME
        // ============================================

        public int Mes { get; set; }

        public int Anio { get; set; }

        // ============================================
        // INDICADORES GENERALES
        // ============================================

        public decimal TotalIngresos { get; set; }

        public int CantidadPedidos { get; set; }

        // ============================================
        // INGRESOS SEMANALES PARA LA GRÁFICA
        // ============================================

        public List<IngresoSemanalViewModel> IngresosSemanales
        {
            get;
            set;
        } = new();

        // ============================================
        // PEDIDOS AGRUPADOS POR ESTADO
        // ============================================

        public Dictionary<string, int> PedidosPorEstado
        {
            get;
            set;
        } = new(StringComparer.OrdinalIgnoreCase)
        {
            ["PendienteAnticipo"] = 0,
            ["Recepcion"] = 0,
            ["EnProceso"] = 0,
            ["Listo"] = 0,
            ["Entregado"] = 0
        };
    }

    // ============================================
    // DATOS DE INGRESOS POR SEMANA
    // ============================================

    public class IngresoSemanalViewModel
    {
        public string Semana { get; set; } = string.Empty;

        public DateTime Inicio { get; set; }

        public DateTime FinExclusivo { get; set; }

        public decimal TotalIngresos { get; set; }
    }
}