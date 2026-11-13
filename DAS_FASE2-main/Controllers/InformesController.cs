using Microsoft.AspNetCore.Mvc;
using DAS_Proyecto.Models;
using DAS_Proyecto.Services;
using DAS_Proyecto.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAS_Proyecto.Controllers
{
    public class InformesController : Controller
    {
        private readonly InformeMensualService _informeService =
            new InformeMensualService();

        [HttpGet]
        public IActionResult Mensual(int? mes, int? anio)
        {
            // ============================================
            // VALIDAR PERIODO SOLICITADO
            // ============================================

            int mesSeleccionado = mes ?? DateTime.Today.Month;
            int anioSeleccionado = anio ?? DateTime.Today.Year;

            if (mesSeleccionado < 1 || mesSeleccionado > 12)
            {
                mesSeleccionado = DateTime.Today.Month;
            }

            if (anioSeleccionado < 2000 ||
                anioSeleccionado > DateTime.Today.Year + 1)
            {
                anioSeleccionado = DateTime.Today.Year;
            }

            // ============================================
            // OBTENER DATOS
            // ============================================

            var pagos = ObtenerPagosParaInforme();

            var pedidos = ObtenerPedidosParaInforme();

            // ============================================
            // CALCULAR INGRESOS
            // ============================================

            decimal totalIngresos =
                _informeService.CalcularTotalIngresos(
                    pagos,
                    mesSeleccionado,
                    anioSeleccionado
                );

            var ingresosSemanales =
                _informeService.CalcularIngresosSemanales(
                    pagos,
                    mesSeleccionado,
                    anioSeleccionado
                );

            // ============================================
            // CALCULAR PEDIDOS
            // ============================================

            int cantidadPedidos =
                _informeService.ContarPedidos(
                    pedidos,
                    mesSeleccionado,
                    anioSeleccionado
                );

            var pedidosPorEstado =
                _informeService.ContarPedidosPorEstado(
                    pedidos,
                    mesSeleccionado,
                    anioSeleccionado
                );

            // ============================================
            // CONSTRUIR VIEWMODEL
            // ============================================

            var informe = new InformeMensualViewModel
            {
                Mes = mesSeleccionado,
                Anio = anioSeleccionado,

                TotalIngresos = totalIngresos,
                CantidadPedidos = cantidadPedidos,

                IngresosSemanales = ingresosSemanales
                    .Select(s => new IngresoSemanalViewModel
                    {
                        Semana = s.Semana,
                        Inicio = s.Inicio,
                        FinExclusivo = s.FinExclusivo,
                        TotalIngresos = s.TotalIngresos
                    })
                    .ToList(),

                PedidosPorEstado = pedidosPorEstado
            };

            return View(informe);
        }

        // ============================================
        // FUENTE TEMPORAL DE PAGOS
        // Pendiente de conectar con IPagoService.
        // ============================================

        private List<Pago> ObtenerPagosParaInforme()
        {
            return new List<Pago>();
        }

        // ============================================
        // FUENTE TEMPORAL DE PEDIDOS
        // Pendiente de conectar con el repositorio
        // de órdenes de trabajo.
        // ============================================

        private List<(DateTime FechaOrden, string Estado)>
            ObtenerPedidosParaInforme()
        {
            return new List<(DateTime FechaOrden, string Estado)>();
        }
    }
}