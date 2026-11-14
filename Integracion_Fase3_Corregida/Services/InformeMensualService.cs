using System;
using System.Collections.Generic;
using System.Linq;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Services
{
    public class InformeMensualService
    {
        // ============================================
        // OBTENER PERIODO DEL INFORME
        // ============================================

        public (DateTime Inicio, DateTime Fin) ObtenerPeriodo(
            int mes,
            int anio)
        {
            if (mes < 1 || mes > 12)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(mes),
                    "El mes debe estar entre 1 y 12."
                );
            }

            if (anio < 2000 || anio > DateTime.Today.Year + 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(anio),
                    "El año seleccionado no es válido."
                );
            }

            DateTime inicio = new DateTime(anio, mes, 1);
            DateTime fin = inicio.AddMonths(1);

            return (inicio, fin);
        }

        // ============================================
        // CALCULAR INGRESOS MENSUALES
        // ============================================

        public decimal CalcularTotalIngresos(
            IEnumerable<Pago> pagos,
            int mes,
            int anio)
        {
            var listaPagos = ValidarPagos(pagos);
            var periodo = ObtenerPeriodo(mes, anio);

            return listaPagos
                .Where(p =>
                    p.FechaPago >= periodo.Inicio &&
                    p.FechaPago < periodo.Fin)
                .Sum(p => p.MontoPago);
        }

        // ============================================
        // CALCULAR INGRESOS SEMANALES
        // Para alimentar la gráfica del informe
        // Las semanas comienzan el lunes.
        // ============================================

        public IReadOnlyList<IngresoSemanal> CalcularIngresosSemanales(
            IEnumerable<Pago> pagos,
            int mes,
            int anio)
        {
            var listaPagos = ValidarPagos(pagos);
            var periodo = ObtenerPeriodo(mes, anio);

            var resultados = new List<IngresoSemanal>();

            // Obtener el lunes de la semana donde comienza el mes.
            int desplazamiento =
                ((int)periodo.Inicio.DayOfWeek + 6) % 7;

            DateTime inicioSemana =
                periodo.Inicio.AddDays(-desplazamiento);

            int numeroSemana = 1;

            while (inicioSemana < periodo.Fin)
            {
                DateTime inicio = inicioSemana < periodo.Inicio
                    ? periodo.Inicio
                    : inicioSemana;

                DateTime finSemana = inicioSemana.AddDays(7);

                DateTime fin = finSemana > periodo.Fin
                    ? periodo.Fin
                    : finSemana;

                decimal total = listaPagos
                    .Where(p =>
                        p.FechaPago >= inicio &&
                        p.FechaPago < fin)
                    .Sum(p => p.MontoPago);

                resultados.Add(new IngresoSemanal(
                    $"Semana {numeroSemana}",
                    inicio,
                    fin,
                    total
                ));

                inicioSemana = finSemana;
                numeroSemana++;
            }

            return resultados;
        }

        // ============================================
        // CONTAR PEDIDOS DEL MES
        // ============================================

        public int ContarPedidos(
            IEnumerable<(DateTime FechaOrden, string Estado)> pedidos,
            int mes,
            int anio)
        {
            var listaPedidos = ValidarPedidos(pedidos);
            var periodo = ObtenerPeriodo(mes, anio);

            return listaPedidos.Count(p =>
                p.FechaOrden >= periodo.Inicio &&
                p.FechaOrden < periodo.Fin
            );
        }

        // ============================================
        // CONTAR PEDIDOS POR ESTADO
        // ============================================

        public Dictionary<string, int> ContarPedidosPorEstado(
            IEnumerable<(DateTime FechaOrden, string Estado)> pedidos,
            int mes,
            int anio)
        {
            var listaPedidos = ValidarPedidos(pedidos);
            var periodo = ObtenerPeriodo(mes, anio);

            var resultado = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["PendienteAnticipo"] = 0,
                ["Recepcion"] = 0,
                ["EnProceso"] = 0,
                ["Listo"] = 0,
                ["Entregado"] = 0
            };

            var pedidosPeriodo = listaPedidos.Where(p =>
                p.FechaOrden >= periodo.Inicio &&
                p.FechaOrden < periodo.Fin);

            // Agrupar pedidos por estado para alimentar el desglose del informe.
            var gruposPorEstado = pedidosPeriodo
                .GroupBy(p => p.Estado.Trim(), StringComparer.OrdinalIgnoreCase);

            foreach (var grupo in gruposPorEstado)
            {
                resultado[grupo.Key] = grupo.Count();
            }

            return resultado;
        }

        // ============================================
        // VALIDAR UNA CANTIDAD YA OBTENIDA
        // Se conserva para compatibilidad.
        // No realiza consultas ni cuenta pedidos.
        // ============================================

        public int ContarPedidos(int cantidadPedidos)
        {
            if (cantidadPedidos < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cantidadPedidos),
                    "La cantidad de pedidos no puede ser negativa."
                );
            }

            return cantidadPedidos;
        }

        // ============================================
        // VALIDAR HISTORIAL DE PAGOS
        // ============================================

        private List<Pago> ValidarPagos(
            IEnumerable<Pago> pagos)
        {
            if (pagos == null)
            {
                throw new ArgumentNullException(nameof(pagos));
            }

            var lista = pagos.ToList();

            if (lista.Any(p => p == null || p.MontoPago <= 0))
            {
                throw new ArgumentException(
                    "El historial contiene pagos inválidos.",
                    nameof(pagos)
                );
            }

            return lista;
        }

        // ============================================
        // VALIDAR DATOS DE PEDIDOS
        // ============================================

        private List<(DateTime FechaOrden, string Estado)> ValidarPedidos(
            IEnumerable<(DateTime FechaOrden, string Estado)> pedidos)
        {
            if (pedidos == null)
            {
                throw new ArgumentNullException(nameof(pedidos));
            }

            var lista = pedidos.ToList();

            if (lista.Any(p =>
                p.FechaOrden == default ||
                string.IsNullOrWhiteSpace(p.Estado)))
            {
                throw new ArgumentException(
                    "El historial contiene pedidos inválidos.",
                    nameof(pedidos)
                );
            }

            return lista;
        }
    }

    // ============================================
    // RESULTADO DE INGRESOS POR SEMANA
    // ============================================

    public sealed record IngresoSemanal(
        string Semana,
        DateTime Inicio,
        DateTime FinExclusivo,
        decimal TotalIngresos
    );
}