using Microsoft.AspNetCore.Mvc;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;

namespace CapaDePresentacion_Web.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteBLL _clienteBll = new ClienteBLL();

        public IActionResult Gestionar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ObtenerPorDni(string dni)
        {
            var cliente = _clienteBll.BuscarCliente(dni);
            if (cliente != null)
                return Json(new { success = true, data = cliente });

            return Json(new { success = false, message = "Cliente no encontrado" });
        }

        [HttpPost]
        public IActionResult Modificar([FromBody] Cliente cliente)
        {
            try
            {
                bool exito = _clienteBll.ModificarCliente(cliente);
                return Json(new { success = exito, message = exito ? "" : "No se pudo actualizar el cliente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Cliente cliente)
        {
            try
            {
                bool exito = _clienteBll.CrearCliente(cliente);
                return Json(new { success = exito, message = exito ? "" : "No se pudo crear el cliente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
