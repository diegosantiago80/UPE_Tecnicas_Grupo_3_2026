using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL.Estrategias;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class VentaBLL
    {
        private readonly ClienteDAL _clienteDAL = new ClienteDAL();
        private readonly VentaDAL _ventaDAL = new VentaDAL();

        public Cliente BuscarClientePorDni(string dni, string dniVendedor = "")
        {
            var cliente = _clienteDAL.BuscarPorDni(dni);
            if (cliente == null)
                throw new Exception("Cliente no registrado. Debe darlo de alta.");

            // un empleado no puede ser cliente de su propia venta
            if (cliente.EsEmpleado && !string.IsNullOrEmpty(dniVendedor) && cliente.Dni == dniVendedor)
                throw new Exception("No es posible registrar una venta a un empleado del sistema. Solicitá a otro vendedor que realice la operación.");

            return cliente;
        }

        public decimal CalcularTotalVenta(Cliente cliente, List<Medicamento> medicamentosVendidos)
        {
            ICalculadorDescuento estrategia;

            // 1. Definimos la estrategia según el perfil del cliente
            if (cliente.ObraSocial.Equals("Particular", StringComparison.OrdinalIgnoreCase))
                estrategia = new DescuentoParticular();
            else
                estrategia = new DescuentoObraSocial();

            decimal totalAcumulado = 0;

            // 2. Iteramos los productos para aplicar la estrategia a cada uno
            foreach (var med in medicamentosVendidos)
            {
                decimal subtotalItem = med.PrecioVenta * med.StockActual;

                // 3. La estrategia recibe el monto y el flag de receta de ese medicamento específico
                totalAcumulado += estrategia.CalcularTotal(subtotalItem, med.RequiereReceta);
            }

            return totalAcumulado;
        }

        // persiste la venta en la BD y devuelve el IdVenta generado
        public int RegistrarVenta(int idCliente, int idUsuarioVendedor, decimal total, List<DetalleVenta> detalle)
        {
            if (detalle == null || detalle.Count == 0)
                throw new Exception("La venta no tiene productos.");

            return _ventaDAL.RegistrarVenta(idCliente, idUsuarioVendedor, total, detalle);
        }
    }
}
