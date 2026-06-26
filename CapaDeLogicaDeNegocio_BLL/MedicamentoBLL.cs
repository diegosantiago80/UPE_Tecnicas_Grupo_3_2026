using CapaDeAccesoADatos_DAL;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CapaDeLogicaDeNegocio_BLL
{
    // delega la persistencia al dal y aplica reglas de negocio
    public class MedicamentoBLL
    {
        private readonly MedicamentoDAL _medicamentoDAL = new MedicamentoDAL();
        private readonly AuditoriaBLL _auditoriaBLL = BLLFactory.CrearAuditoriaBLL();

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

        public List<Medicamento> ObtenerActivos()
        {
            return _medicamentoDAL.ObtenerActivos();
        }

        public List<Medicamento> ObtenerCriticos()
        {
            // limpiar alertas anteriores para reflejar el estado actual del stock
            // sin esto, medicamentos que ya recuperaron stock seguirian apareciendo
            AlertaStockObservador.AlertasActivas.Clear();

            var criticos = _medicamentoDAL.ObtenerCriticos();

            // notificar a los observadores por cada medicamento critico
            foreach (var med in criticos)
                _stockSujeto.EvaluarStock(med);

            return criticos;
        }

        // consulta directa por id, evita traer todos los medicamentos para filtrar en memoria
        public Medicamento? ObtenerPorId(int id)
        {
            return _medicamentoDAL.ObtenerPorId(id);
        }

        /// <summary>
        /// Valida que nombre y descripción cumplan las reglas:
        /// - Solo letras y espacios
        /// - Entre 5 y 20 caracteres
        /// - Máximo un espacio consecutivo
        /// - No espacios al inicio o final
        /// </summary>
        private (bool valido, string mensaje) ValidarNombreYDescripcion(string texto, string campo)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return (false, $"{campo} es obligatorio");

            // Trimear espacios al inicio y final
            texto = texto.Trim();

            // Validar longitud
            if (texto.Length < 5 || texto.Length > 20)
                return (false, $"{campo} debe tener entre 5 y 20 caracteres (actual: {texto.Length})");

            // Validar que solo contiene letras y espacios
            if (!Regex.IsMatch(texto, @"^[A-Za-záéíóúÁÉÍÓÚñÑ\s]+$"))
                return (false, $"{campo} solo puede contener letras y espacios (sin números ni caracteres especiales)");

            // Validar que no hay más de un espacio consecutivo
            if (Regex.IsMatch(texto, @"\s{2,}"))
                return (false, $"{campo} no puede tener más de un espacio consecutivo");

            return (true, "");
        }

        public (bool exito, string mensaje) Agregar(Medicamento m, int idUsuarioLogueado)
        {
            // Validar nombre
            var validacionNombre = ValidarNombreYDescripcion(m.Nombre, "El nombre del medicamento");
            if (!validacionNombre.valido)
                return (false, validacionNombre.mensaje);

            // Validar descripción
            var validacionDescripcion = ValidarNombreYDescripcion(m.Descripcion, "La descripción del medicamento");
            if (!validacionDescripcion.valido)
                return (false, validacionDescripcion.mensaje);

            // Validaciones de precios
            if (m.PrecioVenta <= 0)
                return (false, "El precio de venta debe ser mayor a cero");
            if (m.PrecioCompra <= 0)
                return (false, "El precio de compra debe ser mayor a cero");
            if (m.PrecioVenta < m.PrecioCompra)
                return (false, "El precio de venta no puede ser menor al precio de compra");

            // Validar stock mínimo y asignar default si es necesario
            if (m.StockMinimo < 0)
                return (false, "El stock minimo no puede ser negativo");
            
            // Si no se cargó stock mínimo (0 o no especificado), asignar default de 5
            if (m.StockMinimo == 0)
                m.StockMinimo = 5;

            var resultado = _medicamentoDAL.Agregar(m);

            // Registro auditoria
            _auditoriaBLL.Registrar(idUsuarioLogueado, "Nuevo medicamento", $"Agregó el medicamento: {m.Nombre}");

            return resultado;
        }

        public (bool exito, string mensaje) Modificar(Medicamento m, int idUsuarioLogueado)
        {
            // Validar nombre
            var validacionNombre = ValidarNombreYDescripcion(m.Nombre, "El nombre del medicamento");
            if (!validacionNombre.valido)
                return (false, validacionNombre.mensaje);

            // Validar descripción
            var validacionDescripcion = ValidarNombreYDescripcion(m.Descripcion, "La descripción del medicamento");
            if (!validacionDescripcion.valido)
                return (false, validacionDescripcion.mensaje);

            // Validar precio de venta
            if (m.PrecioVenta <= 0)
                return (false, "El precio de venta debe ser mayor a cero");

            // el precio de compra no viene del formulario (campo deshabilitado)
            // se recupera de la bd para validar que el precio de venta no sea menor
            var medicamentoActual = ObtenerPorId(m.IdMedicamento);
            if (medicamentoActual != null && m.PrecioVenta < medicamentoActual.PrecioCompra)
                return (false, $"El precio de venta (${m.PrecioVenta}) no puede ser menor al precio de compra actual (${medicamentoActual.PrecioCompra})");

            // Si no se cargó stock mínimo (0 o no especificado), asignar default de 5
            if (m.StockMinimo == 0)
                m.StockMinimo = 5;

            var resultado = _medicamentoDAL.Modificar(m);

            // Registro auditoria
            _auditoriaBLL.Registrar(idUsuarioLogueado, "Editar medicamento", $"Modificó el medicamento: {m.Nombre}");

            return resultado;
        }

        public (bool exito, string mensaje) DarDeBaja(int idMedicamento, int idUsuarioLogueado)
        {
            var medicamento = ObtenerPorId(idMedicamento);
            var resultado = _medicamentoDAL.DarDeBaja(idMedicamento);

            // Registro auditoria
            if (medicamento != null)
                _auditoriaBLL.Registrar(idUsuarioLogueado, "Eliminar medicamento", $"Eliminó el medicamento: {medicamento.Nombre}");

            return resultado;
        }
    }
}