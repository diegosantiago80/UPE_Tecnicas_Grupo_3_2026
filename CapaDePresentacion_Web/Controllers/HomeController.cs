using CapaDePresentacion_Web.Filters;
using CapaDePresentacion_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CapaDePresentacion_Web.Controllers
{
    // home es accesible para cualquier usuario logueado, sin restriccion de rol
    [SesionRequerida]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // fallback para rutas todavia sin implementar
        public IActionResult EnConstruccion()
        {
            return View();
        }
    }
}
