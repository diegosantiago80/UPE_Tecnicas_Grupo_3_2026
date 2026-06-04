using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using System.Collections.Generic;

namespace CapaDeLogicaDeNegocio_BLL
{
    // delega la persistencia al dal y aplica reglas de negocio
    public class MedicamentoBLL
    {
        private readonly MedicamentoDAL _medicamentoDAL = new MedicamentoDAL();

        // patron observer: sujeto con observador de alertas registrado
        private readonly StockSujeto _stockSujeto;

        public MedicamentoBLL()
        {
            _stockSujeto = new StockSujeto();
            _stockSujeto.AgregarObservador(new AlertaStockObservador());
        }

        public List<Medicamento> ObtenerTodos()
        {
            return _medicamentoDAL.ObtenerTodos();
        }

        public List<Medicamento> ObtenerCriticos()
        {
            var criticos = _medicamentoDAL.ObtenerCriticos();

            // notificar a los observadores por cada medicamento critico
            foreach (var med in criticos)
                _stockSujeto.EvaluarStock(med);

            return criticos;
        }

        public Medicamento? ObtenerPorId(int id)
        {
            return ObtenerTodos().Find(m => m.IdMedicamento == id);
        }

        public (bool exito, string mensaje) Agregar(Medicamento m)
        {
            // validaciones basicas antes de llegar al dal
            if (string.IsNullOrWhiteSpace(m.Nombre))
                return (false, "El nombre del medicamento es obligatorio");
            if (m.PrecioVenta <= 0)
                return (false, "El precio de venta debe ser mayor a cero");
            if (m.PrecioCompra <= 0)
                return (false, "El precio de compra debe ser mayor a cero");
            if (m.PrecioVenta < m.PrecioCompra)
                return (false, "El precio de venta no puede ser menor al precio de compra");
            if (m.StockMinimo < 0)
                return (false, "El stock minimo no puede ser negativo");

            return _medicamentoDAL.Agregar(m);
        }

        public (bool exito, string mensaje) Modificar(Medicamento m)
        {
            if (string.IsNullOrWhiteSpace(m.Nombre))
                return (false, "El nombre del medicamento es obligatorio");
            if (m.PrecioVenta <= 0)
                return (false, "El precio de venta debe ser mayor a cero");

            // el precio de compra no viene del formulario (campo deshabilitado)
            // se recupera de la bd para validar que el precio de venta no sea menor
            var medicamentoActual = ObtenerPorId(m.IdMedicamento);
            if (medicamentoActual != null && m.PrecioVenta < medicamentoActual.PrecioCompra)
                return (false, $"El precio de venta (${m.PrecioVenta}) no puede ser menor al precio de compra actual (${medicamentoActual.PrecioCompra})");

            return _medicamentoDAL.Modificar(m);
        }

        public (bool exito, string mensaje) DarDeBaja(int idMedicamento)
        {
            return _medicamentoDAL.DarDeBaja(idMedicamento);
        }
    }
}
