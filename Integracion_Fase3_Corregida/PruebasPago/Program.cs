using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAS_Proyecto.Models;
using DAS_Proyecto.Services;
using DAS_Proyecto.Services.Implementations;
using DAS_Proyecto.Repositories.Interfaces;

// ============================================
// CONTADORES DE PRUEBAS
// ============================================

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
// PRUEBAS DE VALIDACIÓN DE PAGOS
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

// PRUEBA 7
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

// PRUEBA 8
var periodoNoviembre = servicioInformes.ObtenerPeriodo(11, 2026);

Probar(
    "Obtener periodo de noviembre de 2026",
    "01/11/2026 - 01/12/2026",
    $"{periodoNoviembre.Inicio:dd/MM/yyyy} - {periodoNoviembre.Fin:dd/MM/yyyy}"
);

// PRUEBA 9
var periodoDiciembre = servicioInformes.ObtenerPeriodo(12, 2026);

Probar(
    "Obtener periodo de diciembre de 2026",
    "01/12/2026 - 01/01/2027",
    $"{periodoDiciembre.Inicio:dd/MM/yyyy} - {periodoDiciembre.Fin:dd/MM/yyyy}"
);

// PRUEBA 10
ProbarExcepcion<ArgumentOutOfRangeException>(
    "Rechazar mes 13",
    () => servicioInformes.ObtenerPeriodo(13, 2026)
);

// PRUEBA 11
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

// PRUEBA 12
Probar(
    "Informe sin pagos registrados",
    "0.00",
    servicioInformes.CalcularTotalIngresos(
        new List<Pago>(), 11, 2026
    ).ToString("F2")
);

// PRUEBA 13
ProbarExcepcion<ArgumentNullException>(
    "Rechazar lista de pagos nula",
    () => servicioInformes.CalcularTotalIngresos(
        null!, 11, 2026
    )
);

// ============================================
// PRUEBAS DEL SERVICIO DE PAGOS
// ============================================

Console.WriteLine("\n========== PRUEBAS DEL SERVICIO DE PAGOS ==========");

// PRUEBA 14: Anticipo válido
var repo1 = new FakePagoRepository();
repo1.AgregarOrden(1, 350m);

var servicio1 = new PagoService(
    repo1,
    new PagoValidacionService()
);

var resultado1 = await servicio1.RegistrarPagoAsync(
    1,
    175m,
    TipoPago.Anticipo,
    new DateTime(2026, 11, 13)
);

Probar(
    "PagoService acepta anticipo válido",
    "True",
    resultado1.Exito.ToString()
);

// PRUEBA 15: Guardar anticipo
Probar(
    "PagoService guarda el anticipo",
    "1",
    repo1.CantidadPagos.ToString()
);

// PRUEBA 16: Anticipo incorrecto
var repo2 = new FakePagoRepository();
repo2.AgregarOrden(1, 350m);

var servicio2 = new PagoService(
    repo2,
    new PagoValidacionService()
);

var resultado2 = await servicio2.RegistrarPagoAsync(
    1,
    100m,
    TipoPago.Anticipo,
    DateTime.Now
);

Probar(
    "PagoService rechaza anticipo incorrecto",
    "False",
    resultado2.Exito.ToString()
);

// PRUEBA 17: No guardar anticipo incorrecto
Probar(
    "PagoService no guarda anticipo incorrecto",
    "0",
    repo2.CantidadPagos.ToString()
);

// PRUEBA 18: Orden inexistente
var repo3 = new FakePagoRepository();

var servicio3 = new PagoService(
    repo3,
    new PagoValidacionService()
);

var resultado3 = await servicio3.RegistrarPagoAsync(
    99,
    175m,
    TipoPago.Anticipo,
    DateTime.Now
);

Probar(
    "PagoService rechaza orden inexistente",
    "False",
    resultado3.Exito.ToString()
);

// PRUEBA 19: Segundo anticipo
var repo4 = new FakePagoRepository();

repo4.AgregarOrden(1, 350m);
repo4.AgregarPagoExistente(
    1, 175m, TipoPago.Anticipo
);

var servicio4 = new PagoService(
    repo4,
    new PagoValidacionService()
);

var resultado4 = await servicio4.RegistrarPagoAsync(
    1,
    175m,
    TipoPago.Anticipo,
    DateTime.Now
);

Probar(
    "PagoService rechaza segundo anticipo",
    "False",
    resultado4.Exito.ToString()
);

// PRUEBA 20: Conservar el anticipo
Probar(
    "PagoService conserva el único anticipo",
    "1",
    repo4.CantidadPagos.ToString()
);

// PRUEBA 21: Abono válido
var repo5 = new FakePagoRepository();

repo5.AgregarOrden(1, 350m);
repo5.AgregarPagoExistente(
    1, 175m, TipoPago.Anticipo
);

var servicio5 = new PagoService(
    repo5,
    new PagoValidacionService()
);

var resultado5 = await servicio5.RegistrarPagoAsync(
    1,
    50m,
    TipoPago.Abono,
    DateTime.Now
);

Probar(
    "PagoService acepta abono válido",
    "True",
    resultado5.Exito.ToString()
);

// PRUEBA 22: Guardar abono
Probar(
    "PagoService guarda el abono",
    "2",
    repo5.CantidadPagos.ToString()
);

// PRUEBA 23: Saldo final
var repo6 = new FakePagoRepository();

repo6.AgregarOrden(1, 350m);
repo6.AgregarPagoExistente(
    1, 175m, TipoPago.Anticipo
);
repo6.AgregarPagoExistente(
    1, 50m, TipoPago.Abono
);

var servicio6 = new PagoService(
    repo6,
    new PagoValidacionService()
);

var resultado6 = await servicio6.RegistrarPagoAsync(
    1,
    125m,
    TipoPago.SaldoFinal,
    DateTime.Now
);

Probar(
    "PagoService acepta saldo final",
    "True",
    resultado6.Exito.ToString()
);

// PRUEBA 24: Registrar pago final
Probar(
    "PagoService registra el pago final",
    "3",
    repo6.CantidadPagos.ToString()
);


// ============================================
// PRUEBAS ADICIONALES DE INFORMES
// ============================================

Console.WriteLine("\n========== PRUEBAS ADICIONALES DE INFORMES ==========");

// PRUEBA 25: Ingresos semanales
var pagosInformeAmpliado = new List<Pago>
{
    new Pago { MontoPago = 40m, FechaPago = new DateTime(2026, 10, 31) },
    new Pago { MontoPago = 100m, FechaPago = new DateTime(2026, 11, 2) },
    new Pago { MontoPago = 50m, FechaPago = new DateTime(2026, 11, 7) },
    new Pago { MontoPago = 25m, FechaPago = new DateTime(2026, 11, 9) },
    new Pago { MontoPago = 70m, FechaPago = new DateTime(2026, 11, 30) }
};

var ingresosSemanales = servicioInformes.CalcularIngresosSemanales(
    pagosInformeAmpliado, 11, 2026);

Probar(
    "Generar seis períodos semanales de noviembre",
    "6",
    ingresosSemanales.Count.ToString()
);

// PRUEBA 26: Total semanal
Probar(
    "Calcular ingresos de la segunda semana",
    "150.00",
    ingresosSemanales
        .First(s => s.Semana == "Semana 2")
        .TotalIngresos.ToString("F2")
);

// PRUEBA 27: Excluir pagos fuera del mes
Probar(
    "Excluir pagos de octubre y sumar ingresos de noviembre",
    "245.00",
    ingresosSemanales.Sum(s => s.TotalIngresos).ToString("F2")
);

// Datos de pedidos de prueba
var pedidosInforme = new List<(DateTime FechaOrden, string Estado)>
{
    (new DateTime(2026, 11, 3), "EnProceso"),
    (new DateTime(2026, 11, 8), "Entregado"),
    (new DateTime(2026, 11, 12), "EnProceso"),
    (new DateTime(2026, 10, 31), "Listo"),
    (new DateTime(2026, 12, 1), "PendienteAnticipo")
};

// PRUEBA 28: Contar pedidos del mes
Probar(
    "Contar únicamente pedidos de noviembre",
    "3",
    servicioInformes
        .ContarPedidos(pedidosInforme, 11, 2026)
        .ToString()
);

// PRUEBA 29: Contar pedidos en proceso
var pedidosPorEstado = servicioInformes.ContarPedidosPorEstado(
    pedidosInforme, 11, 2026);

Probar(
    "Contar pedidos en proceso",
    "2",
    pedidosPorEstado["EnProceso"].ToString()
);

// PRUEBA 30: Contar pedidos entregados
Probar(
    "Contar pedidos entregados",
    "1",
    pedidosPorEstado["Entregado"].ToString()
);

// PRUEBA 31: Estados sin pedidos
Probar(
    "Mostrar cero pedidos recibidos",
    "0",
    pedidosPorEstado["Recepcion"].ToString()
);

// PRUEBA 32: Generación automática del código de pago

bool codigoGeneradoCorrectamente =
    !string.IsNullOrWhiteSpace(resultado1.CodigoPago) &&
    resultado1.CodigoPago.StartsWith("PAG-") &&
    resultado1.CodigoPago.Length == 16;

Probar(
    "Generar código de pago con formato PAG-XXXXXXXXXXXX",
    "True",
    codigoGeneradoCorrectamente.ToString()
);

// ============================================
// RESULTADOS FINALES
// ============================================

Console.WriteLine("\n========== RESULTADOS FINALES ==========");

Console.WriteLine($"Pruebas correctas: {aprobadas}");
Console.WriteLine($"Pruebas fallidas: {fallidas}");
Console.WriteLine($"Total de pruebas: {aprobadas + fallidas}");

if (fallidas > 0)
{
    Environment.ExitCode = 1;
}
else
{
    Console.WriteLine("\nTODAS LAS PRUEBAS FUERON APROBADAS.");
}

// ============================================
// REPOSITORIO SIMULADO PARA LAS PRUEBAS
// ============================================

sealed class FakePagoRepository : IPagoRepository
{
    private readonly List<Pago> _pagos = new();

    private readonly Dictionary<int, decimal> _ordenes = new();

    public int CantidadPagos => _pagos.Count;

    public void AgregarOrden(int id, decimal total)
    {
        _ordenes[id] = total;
    }

    public void AgregarPagoExistente(
        int idOrden,
        decimal monto,
        TipoPago tipo)
    {
        _pagos.Add(new Pago
        {
            IdOrdenTrabajo = idOrden,
            MontoPago = monto,
            TipoPago = tipo,
            FechaPago = DateTime.Now
        });
    }

    public Task<IEnumerable<Pago>> ObtenerTodosAsync()
    {
        return Task.FromResult<IEnumerable<Pago>>(
            _pagos.ToList()
        );
    }

    public Task<Pago?> ObtenerPorIdAsync(int idPago)
    {
        return Task.FromResult(
            _pagos.FirstOrDefault(p => p.IdPago == idPago)
        );
    }

    public Task<IEnumerable<Pago>> ObtenerPorOrdenAsync(
        int idOrdenTrabajo)
    {
        return Task.FromResult<IEnumerable<Pago>>(
            _pagos
                .Where(p => p.IdOrdenTrabajo == idOrdenTrabajo)
                .ToList()
        );
    }

    public Task<Pago?> ObtenerPorCodigoAsync(string codigoPago)
    {
        return Task.FromResult(
            _pagos.FirstOrDefault(p => p.CodigoPago == codigoPago)
        );
    }

    public Task<IEnumerable<OrdenTrabajo>> ObtenerOrdenesParaPagoAsync()
    {
        return Task.FromResult<IEnumerable<OrdenTrabajo>>(
            new List<OrdenTrabajo>()
        );
    }

    public Task<IEnumerable<Pago>> ObtenerPorPeriodoAsync(
        DateTime inicio,
        DateTime finExclusivo)
    {
        return Task.FromResult<IEnumerable<Pago>>(
            _pagos.Where(p => p.FechaPago >= inicio && p.FechaPago < finExclusivo).ToList()
        );
    }

    public Task<IEnumerable<OrdenTrabajo>> ObtenerOrdenesPorPeriodoAsync(
        DateTime inicio,
        DateTime finExclusivo)
    {
        return Task.FromResult<IEnumerable<OrdenTrabajo>>(
            new List<OrdenTrabajo>()
        );
    }

    public Task AgregarAsync(Pago pago)
    {
        _pagos.Add(pago);

        return Task.CompletedTask;
    }

    public Task<decimal> ObtenerTotalPagadoAsync(
        int idOrdenTrabajo)
    {
        decimal total = _pagos
            .Where(p => p.IdOrdenTrabajo == idOrdenTrabajo)
            .Sum(p => p.MontoPago);

        return Task.FromResult(total);
    }

    public Task<bool> ExisteAnticipoAsync(int idOrdenTrabajo)
    {
        bool existe = _pagos.Any(p =>
            p.IdOrdenTrabajo == idOrdenTrabajo &&
            p.TipoPago == TipoPago.Anticipo
        );

        return Task.FromResult(existe);
    }

    public Task<bool> ExisteCodigoAsync(string codigoPago)
    {
        bool existe = _pagos.Any(p =>
            p.CodigoPago == codigoPago
        );

        return Task.FromResult(existe);
    }

    public Task<decimal?> ObtenerTotalOrdenAsync(
        int idOrdenTrabajo)
    {
        decimal? total = _ordenes.TryGetValue(
            idOrdenTrabajo,
            out decimal valor
        )
            ? valor
            : null;

        return Task.FromResult(total);
    }
}