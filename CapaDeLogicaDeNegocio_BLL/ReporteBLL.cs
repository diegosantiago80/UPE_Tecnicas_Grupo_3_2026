using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;

namespace CapaDeLogicaDeNegocio_BLL
{
    //Capa de Lógica de Negocio (BLL - Business Logic Layer).
    //Aquí se aplican las reglas de negocio, cálculos y filtros antes de enviar datos a la Vista.
    public class ReporteBLL
    {
        // En una arquitectura real completa, aquí tendrías una referencia a tu Capa de Datos (DAL):
        private readonly ReporteDAL _reportesDal = new ReporteDAL();

        // Obtiene la lista de ventas aplicando los filtros de negocio.
        public List<Reporte> ObtenerVentasFiltradas(string periodo, DateTime? fechaFiltro)
        {
            // 1. Obtener los datos crudos desde la Capa de Datos (DAL)
            // (Simulando un SELECT * FROM Ventas de SQL Server)
            var ventas = _reportesDal.ObtenerTodasLasVentas();

            // 2. Aplicar la Lógica de Negocio (Los Filtros)
            if (fechaFiltro.HasValue)
            {
                ventas = ventas.Where(v => v.Fecha.Date == fechaFiltro.Value.Date).ToList();
            }
            else if (!string.IsNullOrEmpty(periodo))
            {
                DateTime hoy = DateTime.Today;

                switch (periodo.ToLower())
                {
                    case "diaria":
                        ventas = ventas.Where(v => v.Fecha.Date == hoy).ToList();
                        break;
                    case "semanal":
                        DateTime haceUnaSemana = hoy.AddDays(-7);
                        ventas = ventas.Where(v => v.Fecha.Date >= haceUnaSemana && v.Fecha.Date <= hoy).ToList();
                        break;
                    case "mensual":
                        ventas = ventas.Where(v => v.Fecha.Month == hoy.Month && v.Fecha.Year == hoy.Year).ToList();
                        break;
                }
            }
            return ventas;
        }

        // Este método aplica las reglas de negocio (los filtros) a los datos en bruto
        public List<Reporte> FiltrarVentasComparativa(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            // Regla de Negocio: Si el usuario no ingresó un medicamento, detenemos la lógica 
            // aquí mismo para no pedirle información a la base de datos innecesariamente.
            if (string.IsNullOrEmpty(medicamento))
            {
                return new List<Reporte>();
            }

            // 1. Pedimos los datos crudos a la capa DAL
            var ventas = _reportesDal.ObtenerTodasLasVentas();

            // 2. Filtramos estrictamente por el nombre del medicamento buscado
            var filtradas = ventas.Where(v => v.Medicamento.ToLower().Contains(medicamento.ToLower())).ToList();

            // 3. Aplicamos el filtro de Fecha de Inicio si el usuario la seleccionó
            if (fechaInicio.HasValue)
            {
                filtradas = filtradas.Where(v => v.Fecha.Date >= fechaInicio.Value.Date).ToList();
            }

            // 4. Aplicamos el filtro de Fecha de Fin si el usuario la seleccionó
            if (fechaFin.HasValue)
            {
                filtradas = filtradas.Where(v => v.Fecha.Date <= fechaFin.Value.Date).ToList();
            }

            return filtradas;
        }
    }
}

