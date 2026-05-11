using CapaDeEntidades;
using System.Collections.Generic;
using System.Linq;

namespace CapaDeAccesoADatos_DAL
{
    public class ClienteDAL
    {
        // Lista estática: Es repaso provisorio de la base de datos
        private static List<Cliente> _clientesEnMemoria = new List<Cliente>
        {
            new Cliente(1, "Juan", "Pérez", "11111111", "1144332211", "juan@email.com", "PAMI", true),
            new Cliente(2, "María", "García", "22222222", "1155667788", "maria@email.com", "Particular", true)
        };

      
        public Cliente? BuscarPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return null;
            return _clientesEnMemoria.FirstOrDefault(c => c.Dni.Trim() == dni.Trim());
        }

        public bool Actualizar(Cliente clienteModificado)
        {
            var cliente = _clientesEnMemoria.FirstOrDefault(c => c.IdCliente == clienteModificado.IdCliente);
            if (cliente != null)
            {
                cliente.Dni = clienteModificado.Dni;
                cliente.Nombre = clienteModificado.Nombre;
                cliente.Apellido = clienteModificado.Apellido;
                cliente.Telefono = clienteModificado.Telefono;
                cliente.Email = clienteModificado.Email;
                cliente.ObraSocial = clienteModificado.ObraSocial;
                return true;
            }
            return false;
        }

        public bool Crear(Cliente nuevoCliente)
        {
            try
            {
                nuevoCliente.IdCliente = _clientesEnMemoria.Max(c => c.IdCliente) + 1;
                _clientesEnMemoria.Add(nuevoCliente);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
