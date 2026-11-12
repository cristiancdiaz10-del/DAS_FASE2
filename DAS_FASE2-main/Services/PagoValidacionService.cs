
using System;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Services
{
    public class PagoValidacionService
    {
        public string? ValidarPago(
            decimal totalOrden,
            decimal totalPagado,
            TipoPago tipoPago,
            decimal montoPago,
            bool anticipoRegistrado)
        {
            if (totalOrden <= 0)
                return "El total de la orden debe ser mayor que cero.";

            if (montoPago <= 0)
                return "El monto del pago debe ser mayor que cero.";

            if (totalPagado < 0 || totalPagado > totalOrden)
                return "El historial de pagos de la orden no es válido.";

            if (!Enum.IsDefined(typeof(TipoPago), tipoPago))
                return "El tipo de pago seleccionado no es válido.";

            decimal anticipoEsperado = Math.Round(
                totalOrden * 0.50m, 2);

            if (anticipoRegistrado && totalPagado < anticipoEsperado)
                return "El historial de pagos no contiene el anticipo completo.";

            decimal saldoPendiente = totalOrden - totalPagado;

            if (montoPago > saldoPendiente)
                return "El pago supera el saldo pendiente de la orden.";

            switch (tipoPago)
            {
                case TipoPago.Anticipo:
                    if (anticipoRegistrado || totalPagado > 0)
                        return "Esta orden ya tiene un pago registrado. No puede duplicar el anticipo.";

                    if (montoPago != anticipoEsperado)
                        return $"El anticipo debe ser exactamente el 50 % de la orden: ${anticipoEsperado:N2}.";

                    break;

                case TipoPago.Abono:
                    if (!anticipoRegistrado)
                        return "Debe registrar primero el anticipo inicial.";

                    break;

                case TipoPago.SaldoFinal:
                    if (!anticipoRegistrado)
                        return "No puede liquidar la orden sin registrar el anticipo.";

                    if (montoPago != saldoPendiente)
                        return $"El saldo final debe ser exactamente ${saldoPendiente:N2}.";

                    break;
            }

            return null;
        }
    }
}
