using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL.Estrategias; // Importante para ver tu carpeta nueva
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class VentaBLL
    {
        // Lista temporal de clientes (hasta que hagas el CU de Gestionar Cliente)
        private static List<Cliente> _clientes = new List<Cliente>
        {
            new Cliente(1, "Juan", "Perez", "12345678", "112233", "j@m.com", "OSDE", true),
            new Cliente(2, "Ana", "Gomez", "87654321", "445566", "a@m.com", "Particular", true)
        };

        // Paso 2 y 3 del CU: Buscar cliente por DNI
        public Cliente BuscarClientePorDni(string dni)
        {
            var cliente = _clientes.FirstOrDefault(c => c.Dni == dni);
            if (cliente == null)
            {
                // Mensaje 04 del CU: Cliente no registrado
                throw new Exception("Cliente no registrado. Debe darlo de alta.");
            }
            return cliente;
        }

        // Paso 6 del CU: Calcular el total usando el patrón Strategy
        public decimal ObtenerTotalConDescuento(Cliente cliente, decimal subtotal)
        {
            ICalculadorDescuento estrategia;

            if (cliente.ObraSocial.ToLower() == "particular")
                estrategia = new DescuentoParticular();
            else
                estrategia = new DescuentoObraSocial();

            return estrategia.CalcularTotal(subtotal);
        }
    }
}
