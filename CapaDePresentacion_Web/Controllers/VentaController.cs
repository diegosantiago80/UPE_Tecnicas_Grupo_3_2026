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

            var cliente = _ventaBll.BuscarClientePorDni(request.DniCliente);
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

        [HttpPost]
        public IActionResult ProcesarVenta([FromBody] VentaRequestDTO request)
        {
            // 1. Validaciones iniciales
            if (request?.Lineas == null || request.Lineas.Count == 0)
                return BadRequest("La venta no tiene productos.");

            try
            {
                // 2. Buscamos al cliente usando ClienteDAL a través de VentaBLL
                var cliente = _ventaBll.BuscarClientePorDni(request.DniCliente);

                var listaParaProcesar = new List<Medicamento>();

                foreach (var item in request.Lineas)
                {
                    var medOriginal = _medicamentoBll.ObtenerPorId(item.IdMedicamento);
                    if (medOriginal != null)
                    {
                        // aca crea un objeto nuevo para no sobreescribir el stock real de la lista estática
                        var medVenta = new Medicamento
                        {
                            IdMedicamento = medOriginal.IdMedicamento,
                            Nombre = medOriginal.Nombre,
                            PrecioVenta = item.Precio, 
                            RequiereReceta = medOriginal.RequiereReceta,
                            StockActual = item.Cantidad 
                        };

                        listaParaProcesar.Add(medVenta);
                    }
                }

                // 3. PATRÓN STRATEGY: Calcula el total real según el cliente y los productos
                decimal totalVenta = _ventaBll.CalcularTotalVenta(cliente, listaParaProcesar);

                // 4. PROCESAR STOCK
                bool exito = _medicamentoBll.ProcesarEgresoStock(listaParaProcesar);

                if (exito)
                {
                    return Ok(new { msg = "Venta registrada con éxito", totalCobrado = totalVenta });
                }

                return BadRequest("No se pudo procesar el stock. Verifique si hay stock suficiente.");
            }
            catch (Exception ex)
            {
                // Si el cliente no existe en la DAL, cae acá con el mensaje de la BLL
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
