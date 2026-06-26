using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class CompraBLL
    {
        private readonly CompraDAL _compraDAL = new CompraDAL();
        private readonly AuditoriaBLL _auditoriaBLL = BLLFactory.CrearAuditoriaBLL();

        public (bool exito, int idCompra, string mensaje) RegistrarCompra(Compra compra)
        {
            // validaciones de negocio antes de llegar al dal
            if (compra.IdLaboratorio <= 0)
                return (false, 0, "Debe seleccionar un laboratorio");

            if (compra.IdUsuario <= 0)
                return (false, 0, "El usuario no es valido");

            if (compra.Detalle == null || compra.Detalle.Count == 0)
                return (false, 0, "La compra debe tener al menos un medicamento");

            foreach (var detalle in compra.Detalle)
            {
                if (detalle.Cantidad <= 0)
                    return (false, 0, $"La cantidad del medicamento {detalle.IdMedicamento} debe ser mayor a cero");

                if (detalle.PrecioUnitario <= 0)
                    return (false, 0, $"El precio del medicamento {detalle.IdMedicamento} debe ser mayor a cero");
            }

            // calcular total si no viene cargado
            if (compra.Total == 0)
            {
                foreach (var detalle in compra.Detalle)
                    compra.Total += detalle.Subtotal;
            }

            var resultado = _compraDAL.RegistrarCompra(compra);

            // Registro auditoria
            if (resultado.exito)
                _auditoriaBLL.Registrar(compra.IdUsuario, "Nueva compra", $"Registró la compra N° {resultado.idCompra} por ${compra.Total:F2}");

            return resultado;
        }
    }
}
