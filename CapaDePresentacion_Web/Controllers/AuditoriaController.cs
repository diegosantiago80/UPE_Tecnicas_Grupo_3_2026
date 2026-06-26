using CapaDeLogicaDeNegocio_BLL;
using CapaDePresentacion_Web.Filters;
using Microsoft.AspNetCore.Mvc;

// auditoria: solo el administrador
[SesionRequerida("Administrador")]
public class AuditoriaController : Controller
{
    private readonly AuditoriaBLL _bll = BLLFactory.CrearAuditoriaBLL();

    public IActionResult Index(string? buscarUsuario, string? periodo, string? fechaFiltro)
    {
        if (!string.IsNullOrWhiteSpace(buscarUsuario) &&
            !System.Text.RegularExpressions.Regex.IsMatch(buscarUsuario, @"^[A-Za-z0-9]{5,10}$"))
        {
            buscarUsuario = null;
        }

        DateTime? fechaDesde = null;
        DateTime? fechaHasta = null;

        // si hay filtro activo construimos el rango de fechas segun periodo o fecha exacta
        bool hayFiltro = !string.IsNullOrWhiteSpace(buscarUsuario)
                      || !string.IsNullOrWhiteSpace(periodo)
                      || !string.IsNullOrWhiteSpace(fechaFiltro);

        if (!string.IsNullOrWhiteSpace(fechaFiltro) &&
            DateTime.TryParseExact(fechaFiltro, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime fechaExacta))
        {
            // fecha puntual: desde las 00:00 hasta las 23:59 de ese dia
            fechaDesde = fechaExacta.Date;
            fechaHasta = fechaExacta.Date.AddDays(1).AddSeconds(-1);
        }
        else if (!string.IsNullOrWhiteSpace(periodo))
        {
            DateTime hoy = DateTime.Today;
            switch (periodo.ToLower())
            {
                case "diaria":
                    fechaDesde = hoy;
                    fechaHasta = hoy.AddDays(1).AddSeconds(-1);
                    break;
                case "mensual":
                    fechaDesde = new DateTime(hoy.Year, hoy.Month, 1);
                    fechaHasta = fechaDesde.Value.AddMonths(1).AddSeconds(-1);
                    break;
                case "anual":
                    fechaDesde = new DateTime(hoy.Year, 1, 1);
                    fechaHasta = new DateTime(hoy.Year, 12, 31, 23, 59, 59);
                    break;
            }
        }

        var logs = _bll.ObtenerLogs(fechaDesde, fechaHasta, buscarUsuario, null);

        // pasar los valores actuales a la vista para actualizar el formulario
        ViewBag.BuscarUsuario = buscarUsuario ?? "";
        ViewBag.Periodo = periodo ?? "";
        ViewBag.FechaFiltro = fechaFiltro ?? "";

        return View(logs);
    }
}