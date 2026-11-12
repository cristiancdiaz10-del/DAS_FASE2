
using DAS_Proyecto.Models;
using DAS_Proyecto.Services;

int aprobadas = 0;
int fallidas = 0;

void Probar(
    string nombre,
    string? resultadoEsperado,
    string? resultadoObtenido)
{
    bool correcto = resultadoEsperado == resultadoObtenido;

    if (correcto)
        aprobadas++;
    else
        fallidas++;

    Console.WriteLine(
        $"{(correcto ? "CORRECTA" : "FALLIDA")} - {nombre}");

    if (!correcto)
    {
        Console.WriteLine(
            $"  Esperado: {resultadoEsperado ?? "null"}");

        Console.WriteLine(
            $"  Obtenido: {resultadoObtenido ?? "null"}");
    }
}

void ProbarExcepcion<T>(
    string nombre,
    Action accion) where T : Exception
{
    try
    {
        accion();
        fallidas++;
        Console.WriteLine($"FALLIDA - {nombre}");
        Console.WriteLine($"  Esperado: {typeof(T).Name}");
        Console.WriteLine("  Obtenido: ninguna excepción");
    }
    catch (T)
    {
        aprobadas++;
        Console.WriteLine($"CORRECTA - {nombre}");
    }
    catch (Exception ex)
    {
        fallidas++;
        Console.WriteLine($"FALLIDA - {nombre}");
        Console.WriteLine($"  Esperado: {typeof(T).Name}");
        Console.WriteLine($"  Obtenido: {ex.GetType().Name}");
    }
}

// ============================================
// PRUEBAS DE PAGOS
// ============================================

var servicioPagos = new PagoValidacionService();

Console.WriteLine("========== PRUEBAS DE PAGOS ==========");

// PRUEBA 1
Probar(
    "Anticipo correcto de $175",
    null,
    servicioPagos.ValidarPago(
        350m, 0m, TipoPago.Anticipo, 175m, false)
);

// PRUEBA 2
Probar(
    "Anticipo incorrecto de $100",
    "El anticipo debe ser exactamente el 50 % de la orden: $175.00.",
    servicioPagos.ValidarPago(
        350m, 0m, TipoPago.Anticipo, 100m, false)
);

// PRUEBA 3
Probar(
    "Intento de registrar un segundo anticipo",
    "Esta orden ya tiene un pago registrado. No puede duplicar el anticipo.",
    servicioPagos.ValidarPago(
        350m, 175m, TipoPago.Anticipo, 175m, true)
);

// PRUEBA 4
Probar(
    "Abono válido de $50",
    null,
    servicioPagos.ValidarPago(
        350m, 175m, TipoPago.Abono, 50m, true)
);

// PRUEBA 5
Probar(
    "Intento de pagar más del saldo pendiente",
    "El pago supera el saldo pendiente de la orden.",
    servicioPagos.ValidarPago(
        350m, 225m, TipoPago.Abono, 126m, true)
);

// PRUEBA 6
Probar(
    "Saldo final correcto de $125",
    null,
    servicioPagos.ValidarPago(
        350m, 225m, TipoPago.SaldoFinal, 125m, true)
);

// PRUEBA 7: Enum inválido
Probar(
    "Rechazar tipo de pago inválido",
    "El tipo de pago seleccionado no es válido.",
    servicioPagos.ValidarPago(
        350m, 0m, (TipoPago)99, 175m, false)
);

// ============================================
// PRUEBAS DE INFORMES
// ============================================

var servicioInformes = new InformeMensualService();

Console.WriteLine("\n========== PRUEBAS DE INFORMES ==========");

// PRUEBA 8: Periodo noviembre
var periodoNoviembre = servicioInformes.ObtenerPeriodo(11, 2026);

Probar(
    "Obtener periodo de noviembre de 2026",
    "01/11/2026 - 01/12/2026",
    $"{periodoNoviembre.Inicio:dd/MM/yyyy} - {periodoNoviembre.Fin:dd/MM/yyyy}"
);

// PRUEBA 9: Periodo diciembre
var periodoDiciembre = servicioInformes.ObtenerPeriodo(12, 2026);

Probar(
    "Obtener periodo de diciembre de 2026",
    "01/12/2026 - 01/01/2027",
    $"{periodoDiciembre.Inicio:dd/MM/yyyy} - {periodoDiciembre.Fin:dd/MM/yyyy}"
);

// PRUEBA 10: Mes inválido
ProbarExcepcion<ArgumentOutOfRangeException>(
    "Rechazar mes 13",
    () => servicioInformes.ObtenerPeriodo(13, 2026)
);

// PRUEBA 11: Suma de pagos del periodo
var pagos = new List<Pago>
{
    new Pago
    {
        MontoPago = 175m,
        FechaPago = new DateTime(2026, 11, 5)
    },
    new Pago
    {
        MontoPago = 100m,
        FechaPago = new DateTime(2026, 11, 10)
    },
    new Pago
    {
        MontoPago = 200m,
        FechaPago = new DateTime(2026, 10, 20)
    }
};

decimal totalIngresos = servicioInformes.CalcularTotalIngresos(
    pagos, 11, 2026
);

Probar(
    "Sumar únicamente pagos de noviembre",
    "275.00",
    totalIngresos.ToString("F2")
);

// PRUEBA 12: Lista vacía
Probar(
    "Informe sin pagos registrados",
    "0.00",
    servicioInformes.CalcularTotalIngresos(
        new List<Pago>(), 11, 2026
    ).ToString("F2")
);

// PRUEBA 13: Lista nula
ProbarExcepcion<ArgumentNullException>(
    "Rechazar lista de pagos nula",
    () => servicioInformes.CalcularTotalIngresos(
        null!, 11, 2026
    )
);

// ============================================
// RESULTADOS
// ============================================

Console.WriteLine("\n========== RESULTADOS FINALES ==========");
Console.WriteLine($"Pruebas correctas: {aprobadas}");
Console.WriteLine($"Pruebas fallidas: {fallidas}");
Console.WriteLine($"Total de pruebas: {aprobadas + fallidas}");

if (fallidas > 0)
{
    Environment.ExitCode = 1;
}
