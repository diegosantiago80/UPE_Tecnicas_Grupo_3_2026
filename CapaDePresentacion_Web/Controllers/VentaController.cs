using Microsoft.AspNetCore.Mvc;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CapaDePresentacion_Web.Controllers
{
    public class VentaController : Controller
    {
        private readonly MedicamentoBLL _medicamentoBll = new MedicamentoBLL();
        private readonly VentaBLL _ventaBll = new VentaBLL();

        public IActionResult Registrar()
        {
            ViewBag.Medicamentos = _medicamentoBll.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult CalcularTotalFinal([FromBody] VentaRequestDTO request)
        {
            if (request?.Lineas == null) return BadRequest("Datos inválidos");

            try
            {
                string dniVendedor = HttpContext.Session.GetString("DniUsuario") ?? string.Empty;
                var cliente = _ventaBll.BuscarClientePorDni(request.DniCliente, dniVendedor);

                var medicamentosParaCalcular = new List<Medicamento>();
                foreach (var item in request.Lineas)
                {
                    var med = _medicamentoBll.ObtenerPorId(item.IdMedicamento);
                    if (med != null)
                    {
                        med.PrecioVenta = item.Precio;
                        medicamentosParaCalcular.Add(med);
                    }
                }

                decimal totalFinal = _ventaBll.CalcularTotalVenta(cliente, medicamentosParaCalcular);
                return Json(new { success = true, total = totalFinal });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ProcesarVenta([FromBody] VentaRequestDTO request)
        {
            if (request?.Lineas == null || request.Lineas.Count == 0)
                return BadRequest("La venta no tiene productos.");

            try
            {
                // obtenemos el id del vendedor desde la sesion
                int idVendedor = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
                if (idVendedor == 0)
                    return BadRequest("Sesión expirada. Por favor volvé a iniciar sesión.");

                // buscamos al cliente (incluye validacion de empleado vendiendose a si mismo)
                string dniVendedor = HttpContext.Session.GetString("DniUsuario") ?? string.Empty;
                var cliente = _ventaBll.BuscarClientePorDni(request.DniCliente, dniVendedor);

                // armamos las listas para calcular total y para persistir
                var medicamentosParaTotal = new List<Medicamento>();
                var detalleParaGuardar    = new List<DetalleVenta>();

                foreach (var item in request.Lineas)
                {
                    var medOriginal = _medicamentoBll.ObtenerPorId(item.IdMedicamento);
                    if (medOriginal == null) continue;

                    // objeto para que Strategy calcule el total (usa StockActual como cantidad)
                    medicamentosParaTotal.Add(new Medicamento
                    {
                        IdMedicamento  = medOriginal.IdMedicamento,
                        Nombre         = medOriginal.Nombre,
                        PrecioVenta    = item.Precio,
                        RequiereReceta = medOriginal.RequiereReceta,
                        StockActual    = item.Cantidad
                    });

                    // objeto para el detalle que va a la BD
                    detalleParaGuardar.Add(new DetalleVenta
                    {
                        IdMedicamento  = item.IdMedicamento,
                        Cantidad       = item.Cantidad,
                        PrecioUnitario = item.Precio
                    });
                }

                // calcula el total aplicando descuentos (patron Strategy)
                decimal totalVenta = _ventaBll.CalcularTotalVenta(cliente, medicamentosParaTotal);

                // persiste en BD: inserta en Venta + DetalleVenta y descuenta stock
                int idVenta = _ventaBll.RegistrarVenta(cliente.IdCliente, idVendedor, totalVenta, detalleParaGuardar);

                return Ok(new { msg = "Venta registrada con éxito", idVenta, totalCobrado = totalVenta });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class VentaRequestDTO
    {
        public string DniCliente { get; set; } = string.Empty;
        public string TipoCobro { get; set; } = string.Empty;
        public List<ItemVentaDTO> Lineas { get; set; } = new List<ItemVentaDTO>();
    }

    public class ItemVentaDTO
    {
        public int IdMedicamento { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
