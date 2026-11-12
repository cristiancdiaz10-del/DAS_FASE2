
using System;
using System.Collections.Generic;
using System.Linq;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Services
{
    public class InformeMensualService
    {
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

        public decimal CalcularTotalIngresos(
            IEnumerable<Pago> pagos,
            int mes,
            int anio)
        {
            if (pagos == null)
                throw new ArgumentNullException(nameof(pagos));

            if (pagos.Any(p => p == null || p.MontoPago < 0))
                throw new ArgumentException(
                    "El historial contiene pagos inválidos.",
                    nameof(pagos)
                );

            var periodo = ObtenerPeriodo(mes, anio);

            return pagos
                .Where(p =>
                    p.FechaPago >= periodo.Inicio &&
                    p.FechaPago < periodo.Fin)
                .Sum(p => p.MontoPago);
        }

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
    }
}
