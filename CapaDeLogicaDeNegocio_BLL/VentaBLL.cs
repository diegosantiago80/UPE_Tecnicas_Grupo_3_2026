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

        public Cliente BuscarClientePorDni(string dni)
        {
            var cliente = _clienteDAL.BuscarPorDni(dni);
            if (cliente == null)
            {
                throw new Exception("Cliente no registrado. Debe darlo de alta.");
            }
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
    }
}
