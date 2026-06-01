using Microsoft.AspNetCore.Mvc;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;
using System.Collections.Generic;

namespace CapaPresentacion_Web.Controllers
{
    public class CompraController : Controller
    {
        // factory: creacion centralizada de servicios BLL
        private readonly MedicamentoBLL _medicamentoBLL = BLLFactory.CrearMedicamentoBLL();
        private readonly LaboratorioBLL _laboratorioBLL = BLLFactory.CrearLaboratorioBLL();
        private readonly CompraBLL _compraBLL           = BLLFactory.CrearCompraBLL();

        public IActionResult Registrar()
        {
            // los combos del form se arman desde la bll
            ViewBag.Laboratorios = _laboratorioBLL.ObtenerActivos();
            ViewBag.Medicamentos = _medicamentoBLL.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult Procesar([FromBody] CompraRequestDTO request)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
                return BadRequest("La lista de productos está vacía.");

            // recuperar el id del usuario logueado desde la sesion
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            if (idUsuario == 0)
                return Unauthorized("Debe iniciar sesion para registrar una compra.");

            // armar la entidad compra con su detalle
            var compra = new Compra
            {
                IdLaboratorio = request.IdLaboratorio,
                IdUsuario     = idUsuario,
                Detalle       = new List<DetalleCompra>()
            };

            foreach (var item in request.Items)
            {
                compra.Detalle.Add(new DetalleCompra
                {
                    IdMedicamento  = item.IdMedicamento,
                    Cantidad       = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                });
            }

            var resultado = _compraBLL.RegistrarCompra(compra);

            if (resultado.exito)
                return Ok(new { mensaje = "Compra registrada correctamente", idCompra = resultado.idCompra });

            return StatusCode(500, resultado.mensaje);
        }
    }

    // DTO para recibir la compra desde el cliente
    public class CompraRequestDTO
    {
        public int IdLaboratorio { get; set; }
        public List<ItemCompraDTO> Items { get; set; } = new List<ItemCompraDTO>();
    }

    public class ItemCompraDTO
    {
        public int IdMedicamento { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}